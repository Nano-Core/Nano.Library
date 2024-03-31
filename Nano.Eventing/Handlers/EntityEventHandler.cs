using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
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
    protected virtual DbContext Context { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="context">The <see cref="DbContext"/>.</param>
    public EntityEventHandler(ILogger logger, DbContext context)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public virtual async Task CallbackAsync(EntityEvent @event, bool isRetrying)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        try
        {
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

                    var isSoftDeleted = entityDeleted is IEntityDeletableSoft { IsDeleted: > 0L };

                    if (entityDeleted == null || isSoftDeleted)
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
        }
        catch (Exception ex)
        {
            this.Logger
                .LogError(ex, ex.Message);
        }
    }

    private void SetEntityEventProperties(EntityEvent @event, Type type, object entity)
    {
        foreach (var pair in @event.Data)
        {
            if (pair.Value == null)
            {
                continue;
            }

            var dataProperty = type
                .GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            if (dataProperty == null)
            {
                continue;
            }

            var value = pair.Value.ToString();

            if (value == null)
            {
                continue;
            }

            if (dataProperty.PropertyType == typeof(Guid))
            {
                var guidValue = Guid.Parse(value);

                dataProperty
                    .SetValue(entity, guidValue);
            }
            else if (dataProperty.PropertyType == typeof(DateTime))
            {
                var dateTimeValue = DateTime.Parse(value);

                dataProperty
                    .SetValue(entity, dateTimeValue);
            }
            else if (dataProperty.PropertyType == typeof(DateTimeOffset))
            {
                var dateTimeOffsetValue = DateTimeOffset.Parse(value);

                dataProperty
                    .SetValue(entity, dateTimeOffsetValue);
            }
            else if (dataProperty.PropertyType == typeof(int))
            {
                var intValue = int.Parse(value);

                dataProperty
                    .SetValue(entity, intValue);
            }
            else if (dataProperty.PropertyType == typeof(short))
            {
                var shortValue = short.Parse(value);

                dataProperty
                    .SetValue(entity, shortValue);
            }
            else if (dataProperty.PropertyType == typeof(byte))
            {
                var byteValue = byte.Parse(value);

                dataProperty
                    .SetValue(entity, byteValue);
            }
            else if (dataProperty.PropertyType == typeof(bool))
            {
                var boolValue = bool.Parse(value);

                dataProperty
                    .SetValue(entity, boolValue);
            }
            else if (dataProperty.PropertyType == typeof(float))
            {
                var floatValue = float.Parse(value);

                dataProperty
                    .SetValue(entity, floatValue);
            }
            else if (dataProperty.PropertyType == typeof(double))
            {
                var doubleValue = double.Parse(value);

                dataProperty
                    .SetValue(entity, doubleValue);
            }
            else if (dataProperty.PropertyType == typeof(decimal))
            {
                var decimalValue = decimal.Parse(value);

                dataProperty
                    .SetValue(entity, decimalValue);
            }
            else
            {
                dataProperty
                    .SetValue(entity, value);
            }
        }

    }
}