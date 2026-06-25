namespace TraineeManagement.myapp.Utility
{
    public class RabbitMqSettings
    {
        public string HostName {get; set;} = "localhost"; //(These are default values for fallbacks)
        public int Port {get; set;} = 5672;
        public string UserName {get; set;} = "guest";
        public string Password {get; set;} = "guest";
        public string QueueName {get; set;} = "submission-processing";

        public string DeadLetterExchange {get; set;} = "submission-processing.dlx";
        public string FailedQueue {get; set;} = "submission-processing-failed";
    }
}