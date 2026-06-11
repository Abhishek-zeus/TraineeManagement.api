using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Services;
using TraineeManagement.myapp.DTOs;

/* 
TODO:
1) Replace return type with appropriate response type instead of IActionResult
2) Separate out the global variable with _ like _service and _logger

*/

namespace TraineeManagement.myapp.Controllers 
{
    [ApiController]
    [Route("api/auth/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService service;
        public AuthController(IUserService service)
        {
            this.service = service;
        }

        /*
        TODO:
        1) Add validation for request body, if any required field is missing return bad request with appropriate message
        2) Response code should be 201 for successful registration instead of 200

        */
        [HttpPost("signup")]
        public async Task<IActionResult> RegisterUser(CreateUserRequest request)
        {
            var user = await service.RegisterUser(request);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginRequest request)
        {
            var user = await service.LoginUser(request);
            if(user == null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<User> users = await service.GetAll();
            if(users.Count == 0)
            {
                return NotFound();
            }
            return Ok(users);
        }

    }
}