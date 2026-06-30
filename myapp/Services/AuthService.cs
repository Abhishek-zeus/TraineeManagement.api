using TraineeManagement.myapp.Models;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.myapp.DTOs;
using System.Text;
using System.Security.Claims;       
using Microsoft.IdentityModel.Tokens;
using TraineeManagement.myapp.Exceptions;
using TraineeManagement.myapp.Utility;
using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Mappers;
using TraineeManagement.myapp.Utility;
using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly TokenGeneration _tokenGeneration;
        private readonly IConfiguration _configuration;
        public AuthService(AppDbContext context, IConfiguration configuration, TokenGeneration tokenGeneration)
        {
            _context = context;
            _configuration = configuration;
            _tokenGeneration = tokenGeneration;
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserResponse> RegisterUser(CreateUserRequest request)
        {
            if(request.Role == UserRole.Admin)
            {
                //Exception instantly stops execution of method
                throw new BusinessValidationException("An Admin account cannot be created through public registration.");
            }
            if(request.Role == UserRole.Trainee)
            {
                var traineeExists = await _context.Trainees.AnyAsync(t => t.Email == request.Email);
                if (!traineeExists)
                {
                    throw new BusinessValidationException("Registration failed. No pre-authorized Trainee profile found for this email address.");
                }
            }
            else if (request.Role == UserRole.Mentor)
            {
                var mentorExists = await _context.Mentors.AnyAsync(m => m.Email == request.Email);
                if (!mentorExists)
                {
                    throw new BusinessValidationException("Registration failed. No pre-authorized Mentor profile found for this email address.");
                }
            }
            var user = UserMapper.ToEntity(request);
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);
            if(existing != null)
            {
                return null;
            }


            var hasher = new PasswordHasher<User>();
            String hashedPassword = hasher.HashPassword(user,request.Password);
            user.PasswordHash = hashedPassword;
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return UserMapper.ToResponseDTO(user);
        }

        public async Task<LoginResponse> LoginUser(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
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
            String token = _tokenGeneration.GenerateToken(user);
            return new LoginResponse
            {
                Token = token,
                ExpiresIn = _configuration.GetValue<int>("Jwt:ExpiryMinutes") * 60,
                User = new UserResponse
                {
                    id = user.id,
                    username = user.Username,
                    role = user.Role 
                }
            };
        }

    }

}