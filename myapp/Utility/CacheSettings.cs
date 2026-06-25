namespace TraineeManagement.myapp.Utility
{
    public class CacheSettings
    {
        public int TraineeCacheTtlSeconds {get; set;} = 120;
        public int TaskAssignmentTtlSeconds {get; set;} = 120;
        public int SubmissionSummaryCacheTtlSeconds {get; set;} = 60;
    }
}