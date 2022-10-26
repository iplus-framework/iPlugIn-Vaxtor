using advantech.mes.processapplication;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace advantech.mes.testwebapi.Controllers
{
    public class control : Controller
    {
        [HttpPatch(Name = "control_clear")]
        public IActionResult Patch(FilterClear filter)
        {
            return Content("{}", "text/json", Encoding.UTF8);
        }
    }
}
