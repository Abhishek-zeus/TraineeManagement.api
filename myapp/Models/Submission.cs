using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int TaskAssignmentId { get; set; }
        public string SubmissionUrl { get; set; }
        public string? Notes { get; set; }
        public DateTime SubmittedDate { get; set; }
        public SubmissionStatus Status { get; set; }
        public TaskAssignment TaskAssignment { get; set; }

        //one submission can have multiple files attached
        public ICollection<SubmissionFile> Files {get; set;} = new List<SubmissionFile>();
    }
}