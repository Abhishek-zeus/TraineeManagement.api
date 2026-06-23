using Microsoft.AspNetCore.Mvc;

namespace TraineeManagement.myapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "running",
                application = "Trainee Management API",
                timestamp = DateTime.Now
            });
        }
    }
}