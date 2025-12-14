using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;

namespace Nano.Data.Eventing;

/// <summary>
/// Entity Event Handler.
/// </summary>
public class EntityEventHandler<TIdentity> : IEventingHandler<EntityEvent> 
    where TIdentity : IEquatable<TIdentity>
{
    private readonly BaseDbContext<TIdentity> dbContext;

    /// <inheritdoc />
    public ushort? OverridePrefetchCount { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dbContext">The <see cref="BaseDbContext{TIdentity}"/>.</param>
    public EntityEventHandler(BaseDbContext<TIdentity> dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public virtual async Task CallbackAsync(EntityEvent @event, bool isRetrying)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var type = this.dbContext.Model
            .GetEntityTypes()
            .Select(x => x.ClrType)
            .Where(x => x
                .IsTypeOf(typeof(IEntityIdentity<>)))
            .First(x => x.Name == @event.Type);

        var isGuid = Guid.TryParse(@event.Id.ToString(), out var guid);
        var id = isGuid ? guid : @event.Id;

        var propertyId = type
            .GetProperty(nameof(IEntityIdentity<dynamic>.Id));

        if (propertyId == null)
        {
            throw new NullReferenceException(nameof(propertyId));
        }

        switch (@event.State)
        {
            case "Added":
            {
                var existingEntity = await this.dbContext
                    .FindAsync(type, id);

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
                    .AddAsync(entityAdded);

                break;
            }
            case "Modified":
            {
                var entityModified = await this.dbContext
                    .FindAsync(type, id);

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
                    .FindAsync(type, id);

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
            .SaveChangesWithoutEntityEventsAsync();
    }


    private static void SetEntityEventProperties(EntityEvent @event, IReflect type, object entity)
    {
        foreach (var pair in @event.Data)
        {
            var dataProperty = type
                .GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            if (dataProperty == null)
            {
                continue;
            }

            var value = pair.Value?.ToString();

            if (value == null)
            {
                continue;
            }

            if (dataProperty.PropertyType == typeof(Guid) || dataProperty.PropertyType == typeof(Guid?))
            {
                var guidValue = Guid.Parse(value);

                dataProperty
                    .SetValue(entity, guidValue);
            }
            else if (dataProperty.PropertyType == typeof(TimeSpan) || dataProperty.PropertyType == typeof(TimeSpan?))
            {
                var timeSpanValue = TimeSpan.Parse(value);

                dataProperty
                    .SetValue(entity, timeSpanValue);
            }
            else if (dataProperty.PropertyType == typeof(TimeOnly) || dataProperty.PropertyType == typeof(TimeOnly?))
            {
                var timeOnlyValue = TimeOnly.Parse(value);

                dataProperty
                    .SetValue(entity, timeOnlyValue);
            }
            else if (dataProperty.PropertyType == typeof(DateOnly) || dataProperty.PropertyType == typeof(DateOnly?))
            {
                var dateOnlyValue = DateOnly.Parse(value);

                dataProperty
                    .SetValue(entity, dateOnlyValue);
            }
            else if (dataProperty.PropertyType == typeof(DateTime) || dataProperty.PropertyType == typeof(DateTime?))
            {
                var dateTimeValue = DateTime.Parse(value);

                dataProperty
                    .SetValue(entity, dateTimeValue);
            }
            else if (dataProperty.PropertyType == typeof(DateTimeOffset) || dataProperty.PropertyType == typeof(DateTimeOffset?))
            {
                var dateTimeOffsetValue = DateTimeOffset.Parse(value);

                dataProperty
                    .SetValue(entity, dateTimeOffsetValue);
            }
            else if (dataProperty.PropertyType == typeof(int) || dataProperty.PropertyType == typeof(uint?) || dataProperty.PropertyType == typeof(uint) || dataProperty.PropertyType == typeof(uint?))
            {
                var intValue = int.Parse(value);

                dataProperty
                    .SetValue(entity, intValue);
            }
            else if (dataProperty.PropertyType == typeof(short) || dataProperty.PropertyType == typeof(short?) || dataProperty.PropertyType == typeof(ushort) || dataProperty.PropertyType == typeof(ushort?))
            {
                var shortValue = short.Parse(value);

                dataProperty
                    .SetValue(entity, shortValue);
            }
            else if (dataProperty.PropertyType == typeof(byte) || dataProperty.PropertyType == typeof(byte?))
            {
                var byteValue = byte.Parse(value);

                dataProperty
                    .SetValue(entity, byteValue);
            }
            else if (dataProperty.PropertyType == typeof(bool) || dataProperty.PropertyType == typeof(bool?))
            {
                var boolValue = bool.Parse(value);

                dataProperty
                    .SetValue(entity, boolValue);
            }
            else if (dataProperty.PropertyType == typeof(float) || dataProperty.PropertyType == typeof(float?))
            {
                var floatValue = float.Parse(value);

                dataProperty
                    .SetValue(entity, floatValue);
            }
            else if (dataProperty.PropertyType == typeof(double) || dataProperty.PropertyType == typeof(double?))
            {
                var doubleValue = double.Parse(value);

                dataProperty
                    .SetValue(entity, doubleValue);
            }
            else if (dataProperty.PropertyType == typeof(decimal) || dataProperty.PropertyType == typeof(decimal?))
            {
                var decimalValue = decimal.Parse(value);

                dataProperty
                    .SetValue(entity, decimalValue);
            }
            else if (dataProperty.PropertyType.IsEnum)
            {
                var enumValue = Enum.Parse(dataProperty.PropertyType, value);

                dataProperty
                    .SetValue(entity, enumValue);
            }
            else if (dataProperty.PropertyType == typeof(string))
            {
                dataProperty
                    .SetValue(entity, value);
            }
            else
            {
                dataProperty
                    .SetValue(entity, pair.Value);
            }
        }
    }
}