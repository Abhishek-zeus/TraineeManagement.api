using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.DTOs
{
    public class ProcessingJobResponse
    {
        public int Id { get; set; }
        public string CorrelationId { get; set; } =  string.Empty;
        public int SubmissionId { get; set; }
        public int FileId { get; set; }
        public ProcessingJobStatus Status { get; set; }
        public int Attempts { get; set; }
        public string? ErrorSummary { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}