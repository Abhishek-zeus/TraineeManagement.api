using System.ComponentModel.DataAnnotations;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.DTOs
{
    public class CreateTraineeRequest
    {
        [Required(ErrorMessage="First Name is Required")]
        [MaxLength(50)]
        public String? firstName{get;set;}

        [Required(ErrorMessage="Last Name is Required")]
        [MaxLength(50)]
        public String? lastName{get;set;}

        [Required(ErrorMessage="Email is Required")]
        [EmailAddress(ErrorMessage="Enter a valid email id")]
        public String? email{get;set;}

        [Required(ErrorMessage="TechStack is required")]
        public String? techStack{get;set;}

        [EnumDataType(typeof(ProjectStatus),ErrorMessage="Invalid Status, Allowed are Active, Inactive or Completed")]
        [Required(ErrorMessage="Status is required")]
        public String? status{get;set;} 
        //? is used to depict that variable can be null
    }
}