using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Mappers
{
    public class SubmissionSummaryMapper
    {
        public static SubmissionSummaryResponse ToDTO(Submission s)
        {
            return new SubmissionSummaryResponse
            {
                Id = s.Id,
                Status = s.Status,
                SubmissionUrl = s.SubmissionUrl,
                TaskAssignmentId = s.TaskAssignmentId,
                SubmittedDate = s.SubmittedDate
            };
        }
    }
}