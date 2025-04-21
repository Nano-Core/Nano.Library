using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nano.Data;
using Nano.Eventing.Interfaces;
using Nano.Models.Eventing;
using Nano.Models.Extensions;
using Nano.Models.Helpers;
using Nano.Models.Interfaces;

namespace Nano.Eventing.Handlers;

/// <summary>
/// Entity Event Handler.
/// </summary>
public class EntityEventHandler : IEventingHandler<EntityEvent>
{
    /// <summary>
    /// Logger.
    /// </summary>
    protected virtual ILogger Logger { get; }

    /// <summary>
    /// Context.
    /// </summary>
    protected virtual DefaultDbContext Context { get; }

    /// <inheritdoc />
    public ushort? OverridePrefetchCount { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="context">The <see cref="DbContext"/>.</param>
    public EntityEventHandler(ILogger logger, DefaultDbContext context)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public virtual async Task CallbackAsync(EntityEvent @event, bool isRetrying)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        this.Context.IsEntityEventEnabled = false;

        var type = TypesHelper.GetAllTypes()
            .Where(x => x.IsTypeOf(typeof(IEntityIdentity<>)))
            .First(x => x.Name == @event.Type);

        var isGuid = Guid.TryParse(@event.Id.ToString(), out var guid);
        var id = isGuid ? guid : @event.Id;

        var property = type
            .GetProperty(nameof(IEntityIdentity<dynamic>.Id));

        if (property == null)
        {
            throw new NullReferenceException(nameof(property));
        }

        switch (@event.State)
        {
            case "Added":
            {
                var existingEntity = await this.Context
                    .FindAsync(type, id);

                if (existingEntity != null)
                {
                    this.SetEntityEventProperties(@event, type, existingEntity);

                    this.Context
                        .Update(existingEntity);

                    break;
                }

                var entityAdded = Activator.CreateInstance(type);

                if (entityAdded == null)
                {
                    throw new NullReferenceException(nameof(entityAdded));
                }

                property
                    .SetValue(entityAdded, id);

                this.SetEntityEventProperties(@event, type, entityAdded);

                await this.Context
                    .AddAsync(entityAdded);

                await this.Context
                    .SaveChangesAsync();

                break;
            }
            case "Modified":
            {
                var entityModified = await this.Context
                    .FindAsync(type, id);

                if (entityModified == null)
                {
                    throw new NullReferenceException(nameof(entityModified));
                }

                this.SetEntityEventProperties(@event, type, entityModified);

                this.Context
                    .Update(entityModified);

                await this.Context
                    .SaveChangesAsync();

                break;
            }

            case "Deleted":
            {
                var entityDeleted = await this.Context
                    .FindAsync(type, id);

                if (entityDeleted == null)
                {
                    break;
                }

                this.Context
                    .Remove(entityDeleted);

                await this.Context
                    .SaveChangesAsync();

                break;
            }
        }

        this.Context.IsEntityEventEnabled = true;
    }

    private void SetEntityEventProperties(EntityEvent @event, IReflect type, object entity)
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