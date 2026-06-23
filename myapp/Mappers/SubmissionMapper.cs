using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.DTOs.LearningTask_DTO;

namespace TraineeManagement.myapp.Mappers
{
    public class SubmissionMapper
    {
        public static Submission ToEntity(CreateSubmissionRequest request)
        {
            return new Submission
            {
                TaskAssignmentId = request.TaskAssignmentId,
                SubmissionUrl = request.SubmissionUrl,
                Notes = request.Notes,
                Status = request.Status
            };
        }

        public static CreateSubmissionRequest ToDTO(Submission request)
        {
            return new CreateSubmissionRequest
            {
                TaskAssignmentId = request.TaskAssignmentId,
                SubmissionUrl = request.SubmissionUrl,
                Notes = request.Notes,
                Status = request.Status
            };
        }
    }
}