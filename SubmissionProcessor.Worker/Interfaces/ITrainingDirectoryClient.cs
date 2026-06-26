using TraineeManagement.SubmissionProcessor.Worker.DTOs;

namespace TraineeManagement.SubmissionProcessor.Worker.Interfaces
{
    public interface ITrainingDirectoryClient
    {
        Task<TraineeProcessingProfile> GetTraineeProfileAsync(int traineeId, string correlationId, CancellationToken cancellationToken = default);
    }
}