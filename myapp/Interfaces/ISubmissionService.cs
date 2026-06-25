using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Enums;
using TraineeManagement.myapp.Models;


namespace TraineeManagement.myapp.Interfaces
{
    public interface ISubmissionService
    {
        public Task<CreateSubmissionRequest> SubmitTask(CreateSubmissionRequest request);
        public Task<List<CreateSubmissionRequest>> GetSubmissions();
        public Task<CreateSubmissionRequest> GetById(int id);
        public Task<SubmissionSummaryResponse?> GetSummaryAsync(int submissionId, CancellationToken cancellationToken = default);
    }
}