using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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

        switch (@event.State)
        {
            case "Added":
            {
                var existingEntity = await this.dbContext
                    .FindAsync(type, [id], cancellationToken);

                if (existingEntity != null)
                {
                    SetEntityEventProperties(@event, type, existingEntity);

                    this.dbContext
                        .Update(existingEntity);

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

                await this.dbContext
                    .AddAsync(entityAdded, cancellationToken);

                break;
            }
            case "Modified":
            {
                var entityModified = await this.dbContext
                    .FindAsync(type, [id], cancellationToken);

                if (entityModified == null)
                {
                    throw new NullReferenceException(nameof(entityModified));
                }

                SetEntityEventProperties(@event, type, entityModified);

                this.dbContext
                    .Update(entityModified);

                break;
            }

            case "Deleted":
            {
                var entityDeleted = await this.dbContext
                    .FindAsync(type, [id], cancellationToken);

                if (entityDeleted == null)
                {
                    break;
                }

                this.dbContext
                    .Remove(entityDeleted);

                break;
            }
        }

        await this.dbContext
            .SaveChangesAsync(cancellationToken);
    }


    private static void SetEntityEventProperties(EntityEvent @event, Type type, object entity)
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

            var convertedValue = targetType switch
            {
                { IsEnum: true } => Enum.Parse(targetType, value.ToString()!, true),
                not null when targetType == typeof(Guid) => Guid.Parse(value.ToString()!),
                not null when targetType == typeof(DateTime) => DateTime.Parse(value.ToString()!),
                not null when targetType == typeof(DateTimeOffset) => DateTimeOffset.Parse(value.ToString()!),
                not null when targetType == typeof(TimeSpan) => TimeSpan.Parse(value.ToString()!),
                not null when targetType == typeof(DateOnly) => DateOnly.Parse(value.ToString()!),
                not null when targetType == typeof(TimeOnly) => TimeOnly.Parse(value.ToString()!),
                _ => Convert.ChangeType(value, targetType!)
            };

            property
                .SetValue(entity, convertedValue);
        }
    }
}