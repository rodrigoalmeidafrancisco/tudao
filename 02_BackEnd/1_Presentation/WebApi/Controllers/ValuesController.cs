using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize("PolicyTudao")]
    [Tags("Teste")]
    [Route("values")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {

        }

        [HttpPost("v1/teste1")]
        public async Task<IActionResult> Teste1Async()
        {
            return Ok("Teste");
        }


    }
}
