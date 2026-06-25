namespace TraineeManagement.myapp.Models
{
    public class SubmissionFile
    {
        public int Id {get; set;}

        //Foreign Key
        public int SubmissionId {get; set;}

        //Original name user uploaded
        public string OriginalFileName{get; set;} = string.Empty;

        //Name we used in disk
        public string StorageName{get; set;} = string.Empty;
        
        //MIME type  (e.g. "application/pdf") to check the content type inside file
        public string ContentType {get; set;} = string.Empty;

        public long FileSizeBytes {get; set;}

        // Which user uploaded this file (UserId from JWT)
        public int UploadedByUserId { get; set; }

        //SHA-256 hash of file content to detect duplicates
        public string CheckSum {get; set;} = string.Empty;

        public DateTime UploadedDate {get; set;}

        //Navigation of FK
        public Submission Submission{get; set;} = null!;
    }
}