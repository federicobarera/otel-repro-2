using Microsoft.AspNetCore.Mvc;

namespace otel_repro_2.Controllers
{
    [ApiController]
    [Route("part1")]
    public class Part1Controller : Controller
    {
        private readonly HttpClient client;

        public Part1Controller() {
            this.client = new HttpClient();
        }


        public async Task<IActionResult> Index()
        {
            
            await client.GetAsync("http://localhost:5216/part2");
            return Ok();
        }
    }
}
