using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using Microsoft.EntityFrameworkCore;

using TraineeManagement.myapp.Interfaces;

namespace TraineeManagement.myapp.Services
{
    public class ProcessingJobService : IProcessingJobService
    {
        private readonly AppDbContext _context;
        public ProcessingJobService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProcessingJobResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var job = await _context.ProcessingJobs.FindAsync(new object[] { id }, cancellationToken);
            if (job == null) return null;

            return new ProcessingJobResponse
            {
                Id = job.Id,
                CorrelationId = job.CorrelationId,
                SubmissionId = job.SubmissionId,
                FileId = job.FileId,
                Status = job.Status,
                Attempts = job.Attempts,
                ErrorSummary = job.ErrorSummary,
                CreatedDate = job.CreatedDate,
                StartedDate = job.StartedDate,
                CompletedDate = job.CompletedDate
            };
        }



        public async Task<ProcessingJob> CreateQueuedJobAsync(string messageId, string correlationId, int submissionId, int fileId, CancellationToken cancellationToken = default)
        {
            var job = new ProcessingJob
            {
                MessageId = messageId,
                CorrelationId = correlationId,
                SubmissionId = submissionId,
                FileId = fileId,
                Status = ProcessingJobStatus.Queued,
                Attempts = 0,
                CreatedDate = DateTime.UtcNow
            };

            _context.ProcessingJobs.Add(job);
            await _context.SaveChangesAsync(cancellationToken);
            return job;
        }



        public async Task UpdateJobToFailedAsync(int jobId, string errorSummary, CancellationToken cancellationToken)
        {
            // Retrieve the existing job row from the database
            var job = await _context.ProcessingJobs
                .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);

            if (job == null)
            {
                throw new KeyNotFoundException($"Processing job with ID {jobId} was not found.");
            }

            // Update fields to reflect the failure state
            job.Status = ProcessingJobStatus.Failed;
            job.ErrorSummary = errorSummary;
            job.CompletedDate = DateTime.UtcNow; // Closes out the job timestamp

            await _context.SaveChangesAsync(cancellationToken);
        }


    }
}