/*

TODO: 

1) Use PascalCase for all property names
2) Remove nullable (?) from response fields unless absolutely necessary
3) Change 'status' from string to ProjectStatus enum
*/
namespace TraineeManagement.myapp.DTOs
{
    public class CreateTraineeResponse
    {
        public int? id{get;set;}
        public String? firstName{get;set;}
        public String? lastName{get;set;}
        public String? email{get;set;}
        public String? techStack{get;set;}
        public String? status{get;set;}
        public DateTime CreatedDate{get;set;}
        public DateTime UpdatedDate{get;set;}
    }
}