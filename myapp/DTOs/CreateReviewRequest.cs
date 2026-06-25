using System.ComponentModel.DataAnnotations;
using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.DTOs
{
    public class CreateReviewRequest
    {
        [Required(ErrorMessage="SubmissionId is Required")]
        public int SubmissionId{get;set;}
        [Required(ErrorMessage="MentorId is Required")]
        public int MentorId{get;set;}
        [Required(ErrorMessage="Feedback is Required")]
        public string Feedback{get;set;}
        [Required(ErrorMessage="Score is Required")]
        public int Score{get;set;}
        [Required(ErrorMessage="Status is Required")]
        public ReviewStatus Status{get;set;}
    }
}