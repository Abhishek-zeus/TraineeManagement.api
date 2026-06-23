using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.DTOs.LearningTask_DTO;
using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.Mappers
{
    public class ReviewMapper
    {
        public static Review ToEntity(CreateReviewRequest request)
        {
            return new Review
            {
                SubmissionId = request.SubmissionId,
                MentorId = request.MentorId,
                Feedback = request.Feedback,
                Score = request.Score,
                Status = request.Status
            };
        }

        public static CreateReviewRequest ToDTO(Review request)
        {
            return new CreateReviewRequest
            {
                SubmissionId = request.SubmissionId,
                MentorId = request.MentorId,
                Feedback = request.Feedback,
                Score = request.Score,
                Status = request.Status
            };
        }
    }
}