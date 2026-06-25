using TraineeManagement.SubmissionProcessor.Worker.Models;

namespace TraineeManagement.SubmissionProcessor.Worker.Utility
{
    public class TransientProcessingException : Exception
    {
        public TransientProcessingException() { }
        public TransientProcessingException(string message) : base(message) { }
    }
}