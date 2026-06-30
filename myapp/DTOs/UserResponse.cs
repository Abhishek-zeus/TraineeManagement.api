using System.ComponentModel.DataAnnotations;
using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.DTOs
{
    public class UserResponse
    {
        [Required]
        public int id {get;set;}
        [Required]
        public String username{get;set;}
        [Required]
        public UserRole role {get;set;}
    }
}