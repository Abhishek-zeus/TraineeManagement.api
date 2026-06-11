/*
TODO:
1) Pagination inputs should always be defined (use default values instead)
2) Create Generic PagedResponse<T> class - Ref: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-10.0

*/

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