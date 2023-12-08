using Microsoft.AspNetCore.Mvc;

namespace otel_repro_2.Controllers
{
    [ApiController]
    [Route("part2")]
    public class Part2Controller : Controller
    {
        public Part2Controller() {
        }


        public async Task<IActionResult> Index()
        {
            return Ok();
        }
    }
}
