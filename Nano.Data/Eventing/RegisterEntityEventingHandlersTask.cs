using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Eventing;
using Nano.Data.Abstractions.Eventing.Annotations;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Eventing.Abstractions;

namespace Nano.Data.Eventing;

/// <inheritdoc />
internal sealed class RegisterEntityEventingHandlersTask : IRegisterEntityEventingHandlersTask
{
    private readonly DbContext dbContext;
    private readonly IEventing? eventing;

    /// <summary>
    /// Instantiates and instance of <see cref="RegisterEntityEventingHandlersTask"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <param name="eventing">The <see cref="IEventing"/>.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RegisterEntityEventingHandlersTask(DbContext dbContext, IEventing? eventing = null)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.eventing = eventing;
    }

    /// <inheritdoc />
    public async Task RegisterEntityEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await Task.CompletedTask;

        if (this.eventing == null)
        {
            return;
        }

        var entityTypes = dbContext.Model
            .GetEntityTypes()
            .Where(x =>
                !x.ClrType.IsAbstract &&
                x.ClrType.GetCustomAttributes<SubscribeAttribute>(true).Any());

        foreach (var entityType in entityTypes)
        {
            var eventType = typeof(EntityEvent);

            var eventHandler = serviceProvider
                .GetRequiredService<IEventingHandler<EntityEvent>>();

            var subscribeMethod = eventing
                .GetType()
                .GetMethod(nameof(IEventing.SubscribeAsync));

            subscribeMethod?
                .MakeGenericMethod(eventType)
                .Invoke(eventing, [eventHandler, entityType.ClrType.Name, null, CancellationToken.None]);
        }
    }
}