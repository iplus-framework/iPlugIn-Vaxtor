using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection;
using System.Text;

namespace advantech.mes.testwebapi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class log_message : Controller
    {

        [HttpGet(Name = "log_message_get")]
        public IActionResult Get()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.GetManifestResourceNames().SingleOrDefault(n => n.EndsWith("log_message.json", StringComparison.InvariantCultureIgnoreCase));
            using Stream stream = assembly.GetManifestResourceStream(name);
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
            var content = reader.ReadToEnd();
            return Content(content, "text/json", Encoding.UTF8);
        }
    }
}
