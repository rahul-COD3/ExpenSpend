using ExpenSpend.Domain.DTOs.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExpenSpend.Web.Controllers
{
    [ApiController]
    [Route("api/communication")]
    //[Authorize]
    public class Communication : ControllerBase
    {
        [HttpPost("message")]
        public async Task<IActionResult> MessageAsync(CommunicationDto inputs)
        {
            var result =  "Response is: "+inputs.input;
            return new OkObjectResult(result);
        }
    }
}
