using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Home Controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Index.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IActionResult"/>.</returns>
        public virtual IActionResult Index(CancellationToken cancellationToken = new CancellationToken())
        {
            return this.Ok();
        }

        /// <summary>
        /// Error.
        /// </summary>
        /// <param name="code">The http staus code.</param>
        /// <returns>An <see cref="IActionResult"/>.</returns>
        public virtual IActionResult Error([FromRoute][FromQuery]string code)
        {
            return View(code);
        }

        /// <summary>
        /// Ping.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ActionName("Ping")]
        public virtual IActionResult GetPing()
        {
            return Ok();
        }

        /// <summary>
        /// Options.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpOptions]
        [ActionName("Options")]
        public virtual IActionResult GetOptions()
        {
            return Ok();
        }

        /// <summary>
        /// Returns the Api version requested.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ActionName("Version")]
        public virtual IActionResult GetVersion()
        {
            return this.Ok(this.HttpContext.GetRequestedApiVersion());
        }

        /// <summary>
        /// Sets the language of the consumer.
        /// </summary>
        /// <param name="code">The culture.</param>
        /// <param name="returnUrl">The return url.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        [ActionName("Language")]
        public virtual async Task<IActionResult> SetLanguage([FromQuery][FromHeader(Name = "Accept-Language")][Required]string code, string returnUrl, CancellationToken cancellationToken = new CancellationToken())
        {
            await Task.Factory
                .StartNew(x =>
                {
                    this.Response.Cookies.Append(
                        CookieRequestCultureProvider.DefaultCookieName,
                        CookieRequestCultureProvider.MakeCookieValue(
                            new RequestCulture(code)), 
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(1)
                            }
                    );
                }, null, cancellationToken);

            if (returnUrl != null)
                return LocalRedirect(returnUrl);

            return Ok();
        }
    }
}