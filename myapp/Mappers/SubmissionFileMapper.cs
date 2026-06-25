using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Mappers
{
    public class SubmissionFileMapper
    {
        public static SubmissionFileResponse MapToResponse(SubmissionFile file)
        {
            return new SubmissionFileResponse
            {
                Id = file.Id,
                SubmissionId = file.SubmissionId,
                OriginalFileName = file.OriginalFileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.FileSizeBytes,
                CheckSum = file.CheckSum,
                UploadedByUserId = file.UploadedByUserId,
                UploadedDate = file.UploadedDate
            };
        }
    }
}