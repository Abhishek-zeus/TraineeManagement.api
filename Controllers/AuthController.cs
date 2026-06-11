using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Services;
using TraineeManagement.myapp.DTOs;

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