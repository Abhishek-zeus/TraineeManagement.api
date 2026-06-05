using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage="Status is required")]
        public String? status{get;set;} 
        //? is used to depict that variable can be null
    }
}