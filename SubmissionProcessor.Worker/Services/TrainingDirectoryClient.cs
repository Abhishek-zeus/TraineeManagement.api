using TraineeManagement.SubmissionProcessor.Worker.DTOs;
using TraineeManagement.SubmissionProcessor.Worker.Interfaces;
using System.Net; 
using System.Net.Http.Json;


namespace TraineeManagement.SubmissionProcessor.Worker.Services
{
    public class TrainingDirectoryClient : ITrainingDirectoryClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TrainingDirectoryClient> _logger;
        public TrainingDirectoryClient(HttpClient httpClient, ILogger<TrainingDirectoryClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<TraineeProcessingProfile> GetTraineeProfileAsync(int traineeId, string correlationId, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/directory/trainees/{traineeId}");
            // propagate the same correlation id thats already on the processingJob, that helps in tracing the request across all three services
            request.Headers.Add("X-Correlation-Id", correlationId);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Trainee profile not found. TraineeId: {TraineeId}, CorrelationId {CorrelationId}", traineeId, correlationId);
                return null;
            }

            //if anything other than 404 and 200 comes, this will throws exception and polly needs to handle it (retry)
            response.EnsureSuccessStatusCode(); 
            return await response.Content.ReadFromJsonAsync<TraineeProcessingProfile>(cancellationToken: cancellationToken);
        }
    }
}