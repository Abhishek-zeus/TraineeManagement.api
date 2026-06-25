//Used this to avoid _configuration["FileStorage:StorageRoot"] scattered everywhere by using a strongly typed class. This will bind the whole FileStorage json section.

namespace TraineeManagement.SubmissionProcessor.Worker.Utility
{
    public class FileStorageSettings
    {
        public string StorageRoot {get; set;} = "uploads";
        public long MaxFileSizeBytes {get; set;} = 10485760;
        public List<string> AllowedExtensions {get; set;} = new List<string>();
    }
}