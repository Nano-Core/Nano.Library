using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Eventing.Interfaces;
using Nano.Example.Controllers.Criterias;
using Nano.Example.Models;
using Nano.Example.Models.Events;
using Nano.Services.Interfaces;

namespace Nano.Example.Controllers
{
    /// <inheritdoc />
    public class ExamplesController : DefaultController<ExampleEntity, ExampleCriteria>
    {
        /// <inheritdoc />
        public ExamplesController(ILogger logger, IService service, IEventing eventing)
            : base(logger, service, eventing)
        {
            
        }

        /// <inheritdoc />
        public override async Task<IActionResult> Create([FromBody][FromForm][Required]ExampleEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            return await base.Create(entity, cancellationToken)
                .ContinueWith(x =>
                {
                    // BUG: Handle error / exception in hosting middleware. Test that the middleware logger works at all after all logging changes.
                    if (x.IsFaulted)
                        return x.Result;

                    var @event = new ExampleCreatedEvent
                    {
                        Id = entity.Id,
                        PropertyOne = entity.PropertyOne,
                        PropertyTwo = entity.PropertyTwo
                    };

                    this.Eventing.Publish(@event, cancellationToken: cancellationToken);

                    return x.Result;
                }, cancellationToken);
        }
    }
}