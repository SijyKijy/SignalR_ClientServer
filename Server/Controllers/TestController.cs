using System.Threading.Tasks;
using SignalR_Test.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly IHubContext<TestHub> _testHub;

        public TestController(IHubContext<TestHub> hub)
        {
            _testHub = hub;
        }

        [HttpPost]
        public async Task Create([FromBody] string text) =>
            await _testHub.Clients.All.SendAsync("Notify", text);
    }
}
