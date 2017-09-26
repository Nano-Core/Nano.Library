using Microsoft.AspNetCore.Mvc;

namespace Nano.Services.Globale.Controllers
{
    /// <summary>
    /// Home Controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Index.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/>.</returns>
        public virtual IActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// Error.
        /// </summary>
        /// <param name="code">The http staus code.</param>
        /// <returns>An <see cref="IActionResult"/>.</returns>
        public virtual IActionResult Error([FromRoute]string code)
        {
            return View("Error", code);
        }
    }
}