using TraineeManagement.myapp.Enums;

namespace TraineeManagement.myapp.Models
{
    public class User
    {
        public int id{get;set;}
        public String Username{get;set;}
        public String Email{get;set;}
        public String PasswordHash{get;set;}
        public UserRole Role{get;set;}
        public DateTime CreatedDate{get;set;}
        public DateTime UpdatedDate{get;set;}
    }
}