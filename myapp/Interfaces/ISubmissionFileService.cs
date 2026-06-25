using Microsoft.AspNetCore.Http;
using TraineeManagement.myapp.DTOs;

namespace TraineeManagement.myapp.Interfaces
{
    public interface ISubmissionFileService
    {
        public Task<SubmissionFileResponse> UploadFileAsync(int submissionId, IFormFile file, int uploadedByUserId, CancellationToken cancellationToken = default);

        public Task<SubmissionFileResponse?> GetFileMetadataAsync(int fileId);

        public Task<(Stream FileStream, string OriginalFileName, string ContentType)> DownloadFileAsync(int fileId, CancellationToken cancellationToken = default);

        public Task DeleteFileAsync(int fileId, CancellationToken cancellationToken = default);
    }
}