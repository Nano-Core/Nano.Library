using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
/// <param name="logger">The logger.</param>
public sealed class EntityEventingHandler<TIdentity>(BaseDbContext<TIdentity> dbContext, ILogger<EntityEventingHandler<TIdentity>> logger) : BaseEventHandler<EntityEvent>
    where TIdentity : IEquatable<TIdentity>
{
    private static readonly TimeSpan[] retryDelays = [TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5)];

    private readonly BaseDbContext<TIdentity> dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly ILogger<EntityEventingHandler<TIdentity>> logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
                await this.ApplyAsync(@event, type, propertyId, id, attempt < retryDelays.Length, cancellationToken);

                return;
            }
            catch (MissingParentException exception) when (attempt < retryDelays.Length)
            {
                this.logger
                    .LogInformation("Retrying entity event {TypeName} {Id}, {Navigation} does not exist yet (attempt {Attempt}/{Attempts}).", @event.TypeName, @event.Id, exception.Navigation, attempt + 1, retryDelays.Length);

                this.dbContext.ChangeTracker
                    .Clear();

                await Task.Delay(retryDelays[attempt], cancellationToken);
            }
            catch (DbUpdateException) when (attempt < retryDelays.Length)
            {
                this.dbContext.ChangeTracker
                    .Clear();

                await Task.Delay(retryDelays[attempt], cancellationToken);
            }
        }
    }


    private async Task ApplyAsync(EntityEvent @event, Type type, PropertyInfo propertyId, object id, bool checkParents, CancellationToken cancellationToken)
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

        if (checkParents)
        {
            await this.EnsureParentsExistAsync(cancellationToken);
        }

        await this.dbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Throws when a row, which an added or modified entity references, does not exist. The events of different entities are not ordered, so the row may arrive a moment later. Saving anyway would fail on the foreign key, and EF logs that failure as an error, even when a retry succeeds.
    /// </summary>
    private async Task EnsureParentsExistAsync(CancellationToken cancellationToken)
    {
        var entries = this.dbContext.ChangeTracker
            .Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified)
            .ToArray();

        foreach (var entry in entries)
        {
            var foreignKeys = entry.Metadata
                .GetForeignKeys()
                .Where(x => !x.IsOwnership && x.Properties.Count == 1 && x.PrincipalKey.IsPrimaryKey());

            foreach (var foreignKey in foreignKeys)
            {
                var value = entry
                    .Property(foreignKey.Properties[0].Name)
                    .CurrentValue;

                if (value == null)
                {
                    continue;
                }

                var parent = await this.dbContext
                    .FindAsync(foreignKey.PrincipalEntityType.ClrType, [value], cancellationToken);

                if (parent == null)
                {
                    throw new MissingParentException(foreignKey.DependentToPrincipal?.Name ?? foreignKey.Properties[0].Name);
                }
            }
        }
    }

    private sealed class MissingParentException(string navigation) : Exception
    {
        public string Navigation { get; } = navigation;
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
