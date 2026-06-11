namespace TraineeManagement.myapp.DTOs
{
    public class PagedResponse
    {
        public int? pageNumber{get;set;}
        public int? pageSize{get;set;}
        public int totalRecords{get;set;}
        public List<CreateTraineeRequest> data{get;set;}

        public PagedResponse(int? pageNumber, int? pageSize, int totalRecords, List<CreateTraineeRequest> data)
        {
            this.pageNumber = pageNumber;
            this.pageSize = pageSize;
            this.totalRecords = totalRecords;
            this.data = data;
        }
    }
}