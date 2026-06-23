using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.DTOs.LearningTask_DTO;

namespace TraineeManagement.myapp.Mappers
{
    public class TaskAssignmentMapper
    {
        public static TaskAssignment ToEntity(CreateTaskAssignmentRequest request)
        {
            return new TaskAssignment
            {
                TraineeId = request.TraineeId,
                MentorId = request.MentorId,
                LearningTaskId = request.LearningTaskId,
                DueDate = request.DueDate,
                Remarks = request.Remarks
            };
        }

        public static CreateTaskAssignmentRequest ToDTO(TaskAssignment request)
        {
            return new CreateTaskAssignmentRequest
            {
                TraineeId = request.TraineeId,
                MentorId = request.MentorId,
                LearningTaskId = request.LearningTaskId,
                DueDate = request.DueDate,
                Remarks = request.Remarks
            };
        }
    }
}