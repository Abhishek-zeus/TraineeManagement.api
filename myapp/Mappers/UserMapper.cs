using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;

namespace TraineeManagement.myapp.Mappers
{
    public class UserMapper
    {
        public static User ToEntity(CreateUserRequest request)
        {
            return new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = request.Role,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
        }

        public static UserResponse ToResponseDTO(User request)
        {
            return new UserResponse
            {
                id = request.id,
                username = request.Username,
                role = request.Role,
            };
        }
    }
}