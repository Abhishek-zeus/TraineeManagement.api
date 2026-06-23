using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;

namespace TraineeManagement.myapp.Mappers
{
    public class TraineeMapper
    {
        public static Trainee ToEntity(TraineeDTO request)
        {
            return new Trainee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                TechStack = request.TechStack,
                Status = request.Status
            };
        }

        public static TraineeDTO ToDTO(Trainee request)
        {
            return new TraineeDTO
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                TechStack = request.TechStack,
                Status = request.Status
            };
        }
    }
}