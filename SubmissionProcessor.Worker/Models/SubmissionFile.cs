namespace TraineeManagement.SubmissionProcessor.Worker.Models
{
    public class SubmissionFile
    {
        public int Id {get; set;}

        //Original name user uploaded
        public string OriginalFileName{get; set;} = string.Empty;

        //Name we used in disk
        public string StorageName{get; set;} = string.Empty;

        //SHA-256 hash of file content to detect duplicates
        public string CheckSum {get; set;} = string.Empty;
    }
}