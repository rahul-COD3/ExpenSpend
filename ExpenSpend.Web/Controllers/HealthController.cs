using Microsoft.AspNetCore.Mvc;

namespace ExpenSpend.Web.Controllers
{
    public class HealthController: ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Index()
        {
            return Ok("It's Working!");
        }
    }
}
