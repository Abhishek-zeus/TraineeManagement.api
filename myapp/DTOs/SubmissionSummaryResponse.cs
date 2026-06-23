using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.DTOs
{
    public class SubmissionSummaryResponse
    {
        public int Id{get; set;}
        public SubmissionStatus Status {get; set;}
        public string SubmissionUrl { get; set;} = string.Empty;
        public int TaskAssignmentId {get; set;}
        public DateTime SubmittedDate {get; set;}
    }
}