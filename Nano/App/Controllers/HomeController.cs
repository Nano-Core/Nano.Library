using Microsoft.AspNetCore.Mvc;

namespace Nano.App.Controllers
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
            return View();
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
    }
}