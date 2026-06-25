using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.myapp.DTOs
{
    public class LoginResponse
    {
        [Required]
        public String Token {get;set;}

        [Required]
        public int ExpiresIn {get;set;}

        [Required]
        public UserResponse User {get;set;}
    }
}