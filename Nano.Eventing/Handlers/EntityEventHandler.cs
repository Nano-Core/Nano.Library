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

                    foreach (var pair in @event.Data)
                    {
                        var dataProperty = type
                            .GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance);

                        if (dataProperty == null)
                        {
                            continue;
                        }

                        dataProperty
                            .SetValue(entityAdded, pair.Value);
                    }

                    await this.Context
                        .AddAsync(entityAdded);

                    await this.Context
                        .SaveChangesAsync();

                    break;
                }
                case "Modified":
                    var entityModified = await this.Context
                        .FindAsync(type, id);

                    if (entityModified == null)
                    {
                        throw new NullReferenceException(nameof(entityModified));
                    }

                    foreach (var pair in @event.Data)
                    {
                        var dataProperty = type
                            .GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance);

                        if (dataProperty == null)
                        {
                            continue;
                        }

                        dataProperty
                            .SetValue(entityModified, pair.Value);
                    }

                    this.Context
                        .Update(entityModified);

                    await this.Context
                        .SaveChangesAsync();

                    break;

                case "Deleted":
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

                case "Detached":
                case "Unchanged":
                    break;
            }
        }
        catch (Exception ex)
        {
            this.Logger
                .LogError(ex, ex.Message);
        }
    }
}