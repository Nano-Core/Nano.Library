using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nano.Models;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Extensions;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Home Controller.
    /// Contains method for handling application level operations.
    /// </summary>
    [Authorize(Roles = "administrator, service, writer, reader, guest")]
    public class HomeController : BaseController
    {
        /// <summary>
        /// Gets Ok response.
        /// </summary>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet]
        [Route("index")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public virtual IActionResult Index()
        {
            if (this.Request.IsContentTypeHtml())
                return this.View();

            return this.Ok();
        }

        /// <summary>
        /// Gets the error occured.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>The error.</returns>
        /// <response code="200">Success.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [Route("error")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.OK)]
        public virtual IActionResult Error([FromBody][Required]Error error)
        {
            if (this.Request.IsContentTypeHtml())
                return this.View(error);

            return this.Ok(error);
        }
    }
}