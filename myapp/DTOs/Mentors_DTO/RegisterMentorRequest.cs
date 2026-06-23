using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Enums;
using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.myapp.DTOs.Mentors_DTO
{
    public class RegisterMentorRequest
    {
        [Required(ErrorMessage="Firstname is required")]
        public string? FirstName{get;set;}
        
        [Required(ErrorMessage="Lastname is required")]
        public string? LastName{get;set;}

        [Required(ErrorMessage="Email is required")]
        [EmailAddress(ErrorMessage="Enter a Valid Email id")]
        public string? Email{get;set;}

        [Required(ErrorMessage="Expertise is required")]
        [MaxLength(50)]
        public string? Expertise{get;set;}

        [Required(ErrorMessage="Status is required")]
        [EnumDataType(typeof(MentorStatus), ErrorMessage="Invalid Status, only Active or Inactive is allowed")]
        [MaxLength(50)]
        public string? Status{get;set;}
    }
}