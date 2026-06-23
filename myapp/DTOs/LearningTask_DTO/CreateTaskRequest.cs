using TraineeManagement.myapp.Models;
using System.ComponentModel.DataAnnotations;
using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.DTOs.LearningTask_DTO
{
    public class CreateTaskRequest
    {
        [Required(ErrorMessage="Title is required")]
        [MaxLength(50)]
        public string? Title{get;set;}
        
        [Required(ErrorMessage="Description is required")]
        [MaxLength(50)]
        public string? Description{get;set;}

        [Required(ErrorMessage="TechStack is required")]
        [MaxLength(50)]
        public string? ExpectedTechStack{get;set;}

        [Required(ErrorMessage="DueDate is required")]
        public DateTime DueDate{get;set;}

        [Required(ErrorMessage="Status is required")]
        [EnumDataType(typeof(LearningTaskStatus), ErrorMessage="Invalid Status, only Draft, Published or Closed is allowed")]
        [MaxLength(50)]
        public string? Status{get;set;}
    }
}