using TraineeManagement.SubmissionProcessor.Worker.Models;

namespace TraineeManagement.SubmissionProcessor.Worker.Interfaces
{
    public interface IFileStorageService
    {
        //cancellationToken is used as a listener to instruct the code to stop operation if the user cancels the upload, download anything in the midway by pressing cancel or closing tab

        public Task<string> SaveAsync(Stream fileStream, string storageName, CancellationToken cancellationToken = default);

        public Task<Stream> OpenReadAsync(string storageName, CancellationToken cancellationToken = default);

        public Task<bool> ExistsAsync(string storageName, CancellationToken cancellationToken = default);

        public Task DeleteAsync(string storageName, CancellationToken cancellationToken = default);
    }
}