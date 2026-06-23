using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Mappers;
using TraineeManagement.myapp.Utility;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TraineeManagement.myapp.Services
{
    public class SubmissionFileService : ISubmissionFileService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorage;
        private readonly FileStorageSettings _settings;
        private readonly ILogger<SubmissionFileService> _logger;

        public SubmissionFileService(AppDbContext context, IFileStorageService fileStorage, IOptions<FileStorageSettings> options, ILogger<SubmissionFileService> logger)
        {
            _context = context;
            _fileStorage = fileStorage;
            _settings = options.Value;
            _logger = logger;
        }
        
        // UPLOAD FILE
        public async Task<SubmissionFileResponse> UploadFileAsync(int submissionId, IFormFile file, int uploadedByUserId, CancellationToken cancellationToken = default)
        {
            // Validation step 1 : Submission exists or not
            var submission = await _context.Submissions.FindAsync(submissionId, cancellationToken);
            if(submission == null)
            {
                throw new KeyNotFoundException($"Submission with {submissionId} not found");
            }

            //validation step 2 : File must not be empty 
            if(file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or not provided");
            }

            //validation step 3 : Check file size
            if(file.Length > _settings.MaxFileSizeBytes)
            {
                throw new ArgumentException($"File size {file.Length} bytes exceeds the maximum allowed size of {_settings.MaxFileSizeBytes} bytes.");
            }

            //validation step 4 : Check Extensions
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if(!_settings.AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"File extension '{extension}' is not allowed. Allowed: {string.Join(",",_settings.AllowedExtensions)}");
            }

            //validation step 5 : Check declared contentType
            if(!SubmissionFileUtilities.IsContentTypeAllowed(file.ContentType))
            {
                throw new ArgumentException($"Content type '{file.ContentType}' is not allowed");
            }



            // Generate server-side storage name
            var storageName = $"{Guid.NewGuid()}{extension}";

            //Calculate checksum
            //ms is the temporary RAM (Like a notepad where the data is poured)
            string checkSum;
            using (var ms = new MemoryStream()) //using is used to destroy the allocated RAM memory of MemoryStream and return it to OS as the closing bracket is reached
            {
                await file.CopyToAsync(ms, cancellationToken);
                ms.Position = 0; //move upwards to the position 0
                checkSum = SubmissionFileUtilities.CalculateSHA256Checksum(ms);
            }

            //Save the physical file
            using (var fileStream = file.OpenReadStream())
            {
                await _fileStorage.SaveAsync(fileStream, storageName, cancellationToken);
            }


            //save the metadata to MySQL
            var submissionFile = new SubmissionFile
            {
                SubmissionId = submissionId,
                StorageName = storageName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                CheckSum = checkSum,
                UploadedByUserId = uploadedByUserId,
                UploadedDate = DateTime.UtcNow,
                OriginalFileName = file.FileName
            };
            _context.SubmissionFiles.Add(submissionFile);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("File uploaded. FileId: {FileId}, SubmissionId:{SubmissionId}, OriginalName: {OriginalName}, Size: {Size}",submissionFile.Id, submissionId, submissionFile.OriginalFileName, file.Length);

            return SubmissionFileMapper.MapToResponse(submissionFile);
        }




        // GET METADATA
        public async Task<SubmissionFileResponse?> GetFileMetadataAsync(int fileId)
        {
            var file = await _context.SubmissionFiles.FindAsync(fileId);
            return file == null? null : SubmissionFileMapper.MapToResponse(file);
        }




        // DOWNLOAD FILE
        public async Task<(Stream FileStream, string OriginalFileName, string ContentType)> DownloadFileAsync(int fileId, CancellationToken cancellationToken = default)
        {
            // step 1 : Get metadata from db
            var fileRecord = await _context.SubmissionFiles.FindAsync(fileId, cancellationToken);

            if(fileRecord == null)
            {
                throw new KeyNotFoundException($"File with ID {fileId} not found.");
            }


            // step 2 : Check Physical file exists in storage
            var exists = await _fileStorage.ExistsAsync(fileRecord.StorageName, cancellationToken);

            if (!exists)
            {
                _logger.LogWarning("File metadata found in DB but physicalfile missing. FileId: {FileId}, StorageName: {StorageName}",fileId, fileRecord.StorageName);

                throw new FileNotFoundException("File content not found in storage.");
            }


            //step 3 : open the file stream
            var stream = await _fileStorage.OpenReadAsync(fileRecord.StorageName, cancellationToken);

            _logger.LogInformation("File download requested. FileId; {FileId}", fileId);
            return (stream, fileRecord.OriginalFileName, fileRecord.ContentType);
        }


        //DELETE FILE
        public async Task DeleteFileAsync(int fileId, CancellationToken cancellationToken = default)
        {
            // step 1 : Get metadata from db
            var fileRecord = await _context.SubmissionFiles.FindAsync(fileId, cancellationToken);

            if(fileRecord == null)
            {
                throw new KeyNotFoundException($"File with ID {fileId} not found.");
            }

            // step 2 : Delete from database first
            _context.SubmissionFiles.Remove(fileRecord);
            await _context.SaveChangesAsync(cancellationToken);


            // step 3 : Delete physical file
            await _fileStorage.DeleteAsync(fileRecord.StorageName,cancellationToken);

            _logger.LogInformation("File deleted. FileId: {FileId}, OriginalName:{OriginalName}",fileId, fileRecord.OriginalFileName);
        }

        



    }
}