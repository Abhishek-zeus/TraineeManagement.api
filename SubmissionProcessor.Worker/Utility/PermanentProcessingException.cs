namespace TraineeManagement.SubmissionProcessor.Worker.Utility
{
    public class PermanentProcessingException : Exception
    {
        public PermanentProcessingException(string message) : base(message)
        {
            
        }
    }
}