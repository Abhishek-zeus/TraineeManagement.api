using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public int MentorId { get; set; }
        public string Feedback { get; set; }
        public int Score { get; set; }
        public ReviewStatus Status { get; set; }
        public DateTime ReviewedDate { get; set; }
        public Submission Submission { get; set; }
        public Mentor Mentor { get; set; }
    }
}