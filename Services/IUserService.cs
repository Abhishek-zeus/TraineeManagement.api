using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;

namespace TraineeManagement.myapp.Services
{
    public interface IUserService
    {
        public Task<List<User>> GetAll();
        public Task<User> RegisterUser(CreateUserRequest request);
        public Task<LoginResponse> LoginUser(LoginRequest request);
    }
}