using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.myapp.DTOs
{
    public class CreateTaskAssignmentRequest
    {
        [Required(ErrorMessage="TraineeId is Required")]
        public int TraineeId{get;set;}
        [Required(ErrorMessage="MentorId is Required")]
        public int MentorId{get;set;}
        [Required(ErrorMessage="LearningTaskId is Required")]
        public int LearningTaskId{get;set;}
        [Required(ErrorMessage="DueDate is Required")]
        public DateTime DueDate{get;set;}
        [Required(ErrorMessage="Remarks are Required")]
        public string Remarks{get;set;}
    }
}