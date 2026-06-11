using TraineeManagement.myapp.Models;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.myapp.DTOs;
using System.Text;                  
using System.Security.Claims;       
using Microsoft.IdentityModel.Tokens;
using TraineeManagement.myapp.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.myapp.Services
{
    // TODO: Rename this service to match the controller name and its functionality, for example AuthService
    public class UserService : IUserService
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        public UserService(AppDbContext _context, IConfiguration configuration)
        {
            context = _context;
            this.configuration = configuration;
        }

        public async Task<List<User>> GetAll()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User> RegisterUser(CreateUserRequest request)
        {
            // TODO: Add validation to check if user with same username or email already exists and return appropriate response.
            var user = new User{
                Username = request.Username,
                Email = request.Email,
                Role = request.Role,
                // TODO: Store time in same format and timezone across the application, preferably in UTC.
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            var hasher = new PasswordHasher<User>();
            String hashedPassword = hasher.HashPassword(user,request.Password);
            user.PasswordHash = hashedPassword;
            

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // TODO: Instead of returning entire user object, return only necessary details or a DTO.
            return user;
        }

        public async Task<LoginResponse> LoginUser(LoginRequest request)
        {
            //TODO: Use await for db queries and use async version of method
            var user = context.Users.FirstOrDefault(
                u => u.Username == request.Username
            );
            if(user == null){
                return null;
            }
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if(result == PasswordVerificationResult.Failed)
            {
                return null;
            }
            String token = GenerateToken(user);
            return new LoginResponse
            {
                Token = token,
                // TODO: Store expiry time in configuration and use it here instead of hardcoding.
                ExpiresIn = 3600,
                User = new UserResponse
                {
                    id = user.id,
                    username = user.Username,
                    role = user.Role 
                }
            };
        }

        // TODO: This method can be moved to a separate utility class for token generation and management.
        public String GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var claims = new []
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }

}