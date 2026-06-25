namespace TraineeManagement.myapp.DTOs
{
    public class SubmissionFileResponse
    {
        public int Id {get; set;}
        public int SubmissionId {get; set;}
        public string OriginalFileName{get; set;} = string.Empty;
        public string ContentType {get; set;} = string.Empty;
        public long FileSizeBytes {get; set;}
        public string CheckSum {get; set;} = string.Empty;
        public int UploadedByUserId { get; set; }
        public DateTime UploadedDate {get; set;}
    }
}