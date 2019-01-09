using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Schematic.Core.Mvc
{
    [Route("{culture}/portal")]
    [Authorize]
    public class PortalController : Controller
    {
        [HttpGet]
        public IActionResult Portal()
        {
            return View();
        }
    }
}