using System.ComponentModel.DataAnnotations;
using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.DTOs
{
    public class CreateSubmissionRequest
    {
        [Required(ErrorMessage="TaskAssignmentId is Required")]
        public int TaskAssignmentId{get;set;}
        [Required(ErrorMessage="SubmissionUrl is Required")]
        public string SubmissionUrl{get;set;}
        [Required(ErrorMessage="Notes is Required")]
        public string Notes{get;set;}
        [Required(ErrorMessage="Status is Required")]
        public SubmissionStatus Status{get;set;}
    }
}