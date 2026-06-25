using System.ComponentModel.DataAnnotations;
using TraineeManagement.myapp.Models;



namespace TraineeManagement.myapp.DTOs
{
    public class TraineeDTO 
    {
        [Required(ErrorMessage="First Name is Required")]
        [MaxLength(50)]
        public String FirstName{get;set;}

        [Required(ErrorMessage="Last Name is Required")]
        [MaxLength(50)]
        public String LastName{get;set;}

        [Required(ErrorMessage="Email is Required")]
        [EmailAddress(ErrorMessage="Enter a valid email id")]
        public String Email{get;set;}

        [Required(ErrorMessage="TechStack is required")]
        public String TechStack{get;set;}

        [EnumDataType(typeof(ProjectStatus),ErrorMessage="Invalid Status, Allowed are Active, Inactive or Completed")]
        [Required(ErrorMessage="Status is required")]
        public String Status{get;set;} 
        //? is used to depict that variable can be null
    }
}