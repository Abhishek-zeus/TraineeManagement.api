using TraineeManagement.myapp.DTOs;

namespace TraineeManagement.myapp.Interfaces
{
    public interface IMessagePublisher
    {
        void PublishSubmissionTask(SubmissionProcessingRequest message);
        bool IsConnected();
    }
}