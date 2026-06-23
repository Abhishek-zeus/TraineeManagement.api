using System.ComponentModel.DataAnnotations;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.DTOs
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage="Username is Required")]
        [MaxLength(50)]
        public String Username{get;set;}

        [Required(ErrorMessage="Username is Required")]
        [EmailAddress(ErrorMessage="Invalid Email")]
        [MaxLength(50)]
        public String Email{get;set;}

        [Required(ErrorMessage="Password is Required")]
        [MaxLength(50)]
        public String Password{get;set;}

        [Required(ErrorMessage="Role is Required")]
        [MaxLength(50)]
        public String Role{get;set;}
    }
}