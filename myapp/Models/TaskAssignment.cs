using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.Models
{
    public class TaskAssignment
    {
        public int Id {get; set;}
        public int TraineeId {get; set;}
        public int MentorId {get; set;}
        public int LearningTaskId {get; set;}
        public DateTime AssignedDate {get; set;}
        public DateTime DueDate {get; set;}
        public AssignmentStatus Status {get; set;}
        public string? Remarks {get; set;}
        
        // The following ones allow EF Core Relationships
        public Trainee Trainee {get; set;}
        public Mentor Mentor {get; set;}
        public LearningTask LearningTask {get; set;}

    }
}