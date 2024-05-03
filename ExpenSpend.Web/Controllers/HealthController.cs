using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExpenSpend.Web.Controllers
{
    public class HealthController: ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Index()
        {
            return Ok("It's Working!");
        }

        [HttpPost("save-user-data")]
        public async Task<IActionResult> SaveUserDataAsync([FromBody] UserDataDto userData)
        {
            // do some await task 
            if (userData == null)
            {
                await Task.Delay(100);
                return new BadRequestObjectResult("User data is required");
            }
            return new OkObjectResult(userData);
        }
    }


    public class UserDataDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
