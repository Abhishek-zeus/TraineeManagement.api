namespace TraineeManagement.myapp.Models
{
    public class LearningTask
    {
        public int? id{get; set;}
        public string? Title{get; set;}
        public string? Description{get;set;}
        public string? ExpectedTechStack{get;set;}
        public DateTime DueDate{get;set;}
        public string? Status{get;set;}
        public DateTime CreatedDate{get;set;}
        public DateTime UpdatedDate{get;set;}
    }
}