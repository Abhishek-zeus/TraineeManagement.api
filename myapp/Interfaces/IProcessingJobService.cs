using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Interfaces
{
    public interface IProcessingJobService
    {
        Task<ProcessingJobResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ProcessingJob> CreateQueuedJobAsync(string messageId, string correlationId, int submissionId, int fileId, CancellationToken cancellationToken = default);
        Task UpdateJobToFailedAsync(int jobId, string errorSummary, CancellationToken cancellationToken);

    }
}