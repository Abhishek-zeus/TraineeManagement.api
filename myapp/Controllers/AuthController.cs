using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Services;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.DTOs;


namespace TraineeManagement.myapp.Controllers 
{
    [ApiController]
    [Route("api/auth/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<UserResponse>> RegisterUser(CreateUserRequest request)
        {
            var user = await _service.RegisterUser(request);
            if(user == null)
            {
                return Conflict("A user with this Username/email already exists."); //409
            }
            return Created(user.username,user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginUser(LoginRequest request)
        {
            var user = await _service.LoginUser(request);
            if(user == null)
            {
                return Unauthorized(new {message="Username/Password is Invalid"});
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            List<User> users = await _service.GetAll();
            if(users.Count == 0)
            {
                return Ok(new {message="No User Found"});
            }
            return Ok(users);
        }


        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            // Intentionally crash the endpoint right away
            throw new System.Exception("Database connection timeout simulation!");
        }

    }
}