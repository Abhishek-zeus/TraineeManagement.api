using TraineeManagement.myapp;
using System.ComponentModel.DataAnnotations;
namespace TraineeManagement.myapp.DTOs
{
    public class LoginRequest
    {
        [Required]
        public String Username{get;set;}

        [Required]
        public String Password{get;set;}
    }
}