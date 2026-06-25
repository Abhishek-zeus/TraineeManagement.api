using static System.Collections.IEnumerable;

namespace TraineeManagement.myapp.Models
{
    public class Trainee
    {
        public int id{get;set;}
        public String FirstName{get;set;}
        public String LastName{get;set;}
        public String Email{get;set;}
        public String TechStack{get;set;}
        public String Status{get;set;}
        public DateTime CreatedDate{get;set;}
        public DateTime UpdatedDate{get;set;}
    }
}