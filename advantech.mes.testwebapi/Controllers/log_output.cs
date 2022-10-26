using advantech.mes.processapplication;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace advantech.mes.testwebapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class log_output : Controller
    {
        [HttpPatch(Name = "log_output_patch")]
        public IActionResult Patch(Filter filter)
        {
            return Content("{}", "text/json", Encoding.UTF8);
        }

        [HttpGet(Name = "log_output_get")]
        public IActionResult Get()
        {
            string content = "{\"UID\":1,\"MAC\":0,\"TmF\":1,\"SysTk\":0,\"Fltr\":0,\"TSt\":1666013198,\"TEnd\":1666065600,\"Amt\":20,\"Total\":200,\"TLst\":1666013198,\"TFst\":1666086644}";
            return Content(content, "text/json", Encoding.UTF8);
        }
    }
}
