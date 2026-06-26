namespace TraineeManagement.SubmissionProcessor.Worker.DTOs
{
    public class TraineeProcessingProfile
    {
        public int TraineeId{get; set;}
        public string FullName {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string EnrollmentStatus {get; set;} = string.Empty;
    }
}