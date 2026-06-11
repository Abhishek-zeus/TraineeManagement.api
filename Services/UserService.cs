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
            var user = new User{
                Username = request.Username,
                Email = request.Email,
                Role = request.Role,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            var hasher = new PasswordHasher<User>();
            String hashedPassword = hasher.HashPassword(user,request.Password);
            user.PasswordHash = hashedPassword;
            

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<LoginResponse> LoginUser(LoginRequest request)
        {
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
                ExpiresIn = 3600,
                User = new UserResponse
                {
                    id = user.id,
                    username = user.Username,
                    role = user.Role 
                }
            };
        }

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