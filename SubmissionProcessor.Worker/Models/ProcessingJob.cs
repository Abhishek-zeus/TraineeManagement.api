using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TraineeManagement.SubmissionProcessor.Worker.Enums;


namespace TraineeManagement.SubmissionProcessor.Worker.Models
{
    public class ProcessingJob
    {
        public int Id {get; set;}
        public string MessageId {get; set;} =  string.Empty;
        public string CorrelationId {get; set;} =  string.Empty;
        public int SubmissionId {get; set;}
        public int FileId {get; set;}
        public ProcessingJobStatus Status {get; set;} = ProcessingJobStatus.Queued;

        public int Attempts {get; set;}
        public string? ErrorSummary {get; set;}

        public DateTime CreatedDate {get; set;}
        public DateTime? StartedDate {get; set;}
        public DateTime CompletedDate {get; set;}

    }
}