using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Enums;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;
using Nano.Web.Controllers;
using NanoCore.Example.Models;
using NanoCore.Example.Models.Criterias;
using NanoCore.Example.Models.Events;

namespace NanoCore.Example.Controllers
{
    /// <inheritdoc />
    public class ExamplesController : DefaultController<ExampleEntity, ExampleQueryCriteria>
    {
        /// <inheritdoc />
        public ExamplesController(ILogger logger, IService service, IEventing eventing)
            : base(logger, service, eventing)
        {
            
        }

        /// <inheritdoc />
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody][FromForm][Required]ExampleEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            var task = base.Create(entity, cancellationToken);

            await task.ContinueWith(x =>
            {
                if (x.IsFaulted || x.IsCanceled)
                    return;

                var @event = new ExampleCreatedEvent
                {
                    Id = entity.Id,
                    PropertyOne = entity.PropertyOne,
                    PropertyTwo = entity.PropertyTwo
                };

                this.Eventing.Publish(@event, Topology.Direct);
            }, cancellationToken);

            return await task;
        }
    }
}