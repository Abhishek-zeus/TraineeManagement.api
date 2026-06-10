using TraineeManagement.myapp.Models;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Data;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.myapp.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext context;
        public UserService(AppDbContext _context)
        {
            context = _context;
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

        public async Task<User> LoginUser(LoginRequest request)
        {
            var user = context.Users.FirstOrDefault(
                u => u.Username == request.Username
            );
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if(result == PasswordVerificationResult.Success)
            {
                return user;
            }
            return null;
        }
    }

}