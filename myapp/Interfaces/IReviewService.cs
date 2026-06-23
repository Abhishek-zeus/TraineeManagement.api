using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Enums;


namespace TraineeManagement.myapp.Interfaces
{
    public interface IReviewService
    {
        public Task<CreateReviewRequest> ReviewTask(CreateReviewRequest request);
        public Task<List<CreateReviewRequest>> GetReviews();
        public Task<CreateReviewRequest> GetById(int id);
    }
}