using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;

namespace TraineeManagement.myapp.Interfaces
{
    public interface IAuthService
    {
        public Task<List<User>> GetAll();
        public Task<UserResponse> RegisterUser(CreateUserRequest request);
        public Task<LoginResponse> LoginUser(LoginRequest request);
    }
}