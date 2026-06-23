

namespace TraineeManagement.myapp.DTOs
{
    public class PagedResponse<T>
    {
        public int PageNumber{get;set;}
        public int PageSize{get;set;}
        public int TotalRecords{get;set;}
        public List<T> Data{get;set;}

        public PagedResponse(int? pageNumber, int? pageSize, int totalRecords, List<T> data)
        {
            this.PageNumber = pageNumber ?? 1;
            this.PageSize = pageSize ?? 10;

            if(this.PageNumber < 1) this.PageNumber = 1;
            if(this.PageSize < 1) this.PageSize = 10;

            this.TotalRecords = totalRecords;
            this.Data = data ?? new List<T>();
        }
    }
}