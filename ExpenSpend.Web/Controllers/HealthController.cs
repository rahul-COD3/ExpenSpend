using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExpenSpend.Web.Controllers
{
    public class HealthController: ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet("test")]
        public IActionResult Index()
        {
            _logger.LogInformation("This is an information log message");
            _logger.LogWarning("This is a warning log message");
            _logger.LogError("This is an error log message");
            _logger.LogDebug("This is a debug log message");
            _logger.LogTrace("This is a trace log message");
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
