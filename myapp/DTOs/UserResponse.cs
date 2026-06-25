using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.myapp.DTOs
{
    public class UserResponse
    {
        [Required]
        public int id {get;set;}
        [Required]
        public String username{get;set;}
        [Required]
        public String role {get;set;}
    }
}