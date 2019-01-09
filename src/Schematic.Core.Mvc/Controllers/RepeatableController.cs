using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schematic.Core;

namespace Schematic.Core.Mvc
{
    [Route("{culture}/repeatable/{parent}/[controller]")]
    [Authorize]
    public class RepeatableController<T> : Controller where T : class, new()
    {
        public string ResourceType = typeof(T).Name;

        [Route("create")]
        [HttpGet]
        public virtual IActionResult Create(string parent)
        {
            if (!User.IsAuthorized(typeof(T))) 
            {
                return Unauthorized();
            }

            string view = "~/Views/" + parent + "/EditorTemplates/" + ResourceType + ".cshtml";

            return PartialView(view, new T());
        }
    }
}