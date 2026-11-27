using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;

namespace Nano.Data.Eventing;

/// <summary>
/// Handles <see cref="EntityEvent"/> instances by applying the events to the database context of type <see cref="BaseDbContext{TIdentity}"/>.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity. Must implement <see cref="IEquatable{T}"/>.</typeparam>
/// <param name="dbContext">The <see cref="BaseDbContext{TIdentity}"/> used to apply entity events.</param>
public sealed class EntityEventingHandler<TIdentity>(BaseDbContext<TIdentity> dbContext) : BaseEventHandler<EntityEvent>
    where TIdentity : IEquatable<TIdentity>
{
    private static readonly TimeSpan[] retryDelays = [TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5)];

    private readonly BaseDbContext<TIdentity> dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    /// <inheritdoc />
    public override async Task CallbackAsync(EntityEvent @event, bool isRedelivered, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var type = this.dbContext.Model
            .GetEntityTypes()
            .Select(x => x.ClrType)
            .Where(x => x
                .IsTypeOf(typeof(IEntityIdentity<>)))
            .First(x => x.Name == @event.TypeName);

        var isGuid = Guid.TryParse(@event.Id.ToString(), out var guid);
        var id = isGuid ? guid : @event.Id;

        var propertyId = type
            .GetProperty(nameof(IEntityIdentity<>.Id));

        if (propertyId == null)
        {
            throw new NullReferenceException(nameof(propertyId));
        }

        for (var attempt = 0; ; attempt++)
        {
            try
            {
                await this.ApplyAsync(@event, type, propertyId, id, cancellationToken);

                return;
            }
            catch (DbUpdateException) when (attempt < retryDelays.Length)
            {
                this.dbContext.ChangeTracker
                    .Clear();

                await Task.Delay(retryDelays[attempt], cancellationToken);
            }
        }
    }


    private async Task ApplyAsync(EntityEvent @event, Type type, PropertyInfo propertyId, object id, CancellationToken cancellationToken)
    {
        var existingEntity = await this.dbContext
            .FindAsync(type, [id], cancellationToken);

        switch (@event.State)
        {
            case "Added":
            case "Modified":
            {
                if (existingEntity != null)
                {
                    if (@event.State == "Modified" && !IsOutdated(@event, type, existingEntity))
                    {
                        SetEntityEventProperties(@event, type, existingEntity);
                        SetEntityEventUpdatedAt(@event, type, existingEntity);

                        this.dbContext
                            .Update(existingEntity);
                    }

                    break;
                }

                var entityAdded = Activator.CreateInstance(type);

                if (entityAdded == null)
                {
                    throw new NullReferenceException(nameof(entityAdded));
                }

                propertyId
                    .SetValue(entityAdded, id);

                SetEntityEventProperties(@event, type, entityAdded);
                SetEntityEventUpdatedAt(@event, type, entityAdded);

                await this.dbContext
                    .AddAsync(entityAdded, cancellationToken);

                break;
            }

            case "Deleted":
            {
                if (existingEntity == null)
                {
                    break;
                }

                this.dbContext
                    .Remove(existingEntity);

                break;
            }
        }

        await this.dbContext
            .SaveChangesAsync(cancellationToken);
    }

    private static bool IsOutdated(EntityEvent @event, IReflect type, object entity)
    {
        ArgumentNullException.ThrowIfNull(@event);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(entity);

        var property = type
            .GetProperty(nameof(IEntityReadOnly<>.UpdatedAt), BindingFlags.Public | BindingFlags.Instance);

        if (property == null || !@event.UpdatedAt.HasValue)
        {
            return false;
        }

        var current = (DateTimeOffset)property.GetValue(entity)!;

        return @event.UpdatedAt.Value <= current;
    }
    private static void SetEntityEventUpdatedAt(EntityEvent @event, IReflect type, object entity)
    {
        ArgumentNullException.ThrowIfNull(@event);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(entity);

        var property = type
            .GetProperty(nameof(IEntityReadOnly<>.UpdatedAt), BindingFlags.Public | BindingFlags.Instance);

        if (property == null || !property.CanWrite || !@event.UpdatedAt.HasValue)
        {
            return;
        }

        property
            .SetValue(entity, @event.UpdatedAt.Value);
    }
    private static void SetEntityEventProperties(EntityEvent @event, IReflect type, object entity)
    {
        ArgumentNullException.ThrowIfNull(@event);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(entity);

        foreach (var (key, value) in @event.Data)
        {
            var property = type
                .GetProperty(key, BindingFlags.Public | BindingFlags.Instance);

            if (property == null || !property.CanWrite)
            {
                continue;
            }

            if (value == null)
            {
                property
                    .SetValue(entity, null);

                continue;
            }

            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            var valueString = value.ToString();

            if (valueString == null)
            {
                throw new NullReferenceException(nameof(valueString));
            }

            var convertedValue = targetType switch
            {
                { IsEnum: true } => Enum.Parse(targetType, valueString, true),
                not null when targetType == typeof(Guid) => Guid.Parse(valueString),
                not null when targetType == typeof(DateTime) => DateTime.Parse(valueString),
                not null when targetType == typeof(DateTimeOffset) => DateTimeOffset.Parse(valueString),
                not null when targetType == typeof(TimeSpan) => TimeSpan.Parse(valueString),
                not null when targetType == typeof(DateOnly) => DateOnly.Parse(valueString),
                not null when targetType == typeof(TimeOnly) => TimeOnly.Parse(valueString),
                _ => Convert.ChangeType(value, targetType!)
            };

            property
                .SetValue(entity, convertedValue);
        }
    }
}
