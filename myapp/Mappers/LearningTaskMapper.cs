using TraineeManagement.myapp.DTOs.LearningTask_DTO;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Mappers
{
    public class LearningTaskMapper
    {
        public static CreateTaskRequest ConvertToDTO(LearningTask task)
        {
            CreateTaskRequest response = new()
            {
                Title = task.Title,
                Description = task.Description,
                ExpectedTechStack = task.ExpectedTechStack,
                DueDate = task.DueDate,
                Status = task.Status
            };
            return response;
        }

        public static LearningTask ConvertToModel(CreateTaskRequest request)
        {
            var task = new LearningTask()
            {
                Title = request.Title,
                Description = request.Description,
                ExpectedTechStack = request.ExpectedTechStack,
                DueDate = request.DueDate,
                Status = request.Status,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            return task;
        }
    }
}