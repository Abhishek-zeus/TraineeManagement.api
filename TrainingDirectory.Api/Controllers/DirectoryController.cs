using System.Net.Cache;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.TrainingDirectory.Api.DTOs;

namespace TraineeManagement.TrainingDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/directory")]
    public class DirectoryController : ControllerBase
    {
        private readonly ILogger<DirectoryController> _logger;

        public DirectoryController(ILogger<DirectoryController> logger)
        {
            _logger = logger;
        }

        [HttpGet("trainees/{traineeId}")]
        public async Task<ActionResult<TraineeProcessingProfileResponse>> GetTraineeProfile(int traineeId, CancellationToken cancellationToken)
        {
            var correlationId = Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? "none";
            _logger.LogInformation("Directory lookup received. TraineeId: {TraineeId}, CorrelationId: {CorrelationId}", traineeId, correlationId);

            //---Deliberate test hooks for process resilience---
            switch (traineeId)
            {
                case 9999: // Simulates a valid request for a trainee that genuinely doesn't exist
                    return NotFound(new {message = $"Trainee {traineeId} not found."});
                case 8888: // Simulates a slow/hanging downstream dependency (used to trigger timeout + retry)
                    _logger.LogWarning("Simulating a slow response for TraineeId: {traineeId}", traineeId);
                    await Task.Delay(5000, cancellationToken);
                    break;
                case 7777: //Simulates a downstream failure (used to trigger retry + circuit breaker)
                    _logger.LogWarning("Simulating a server error for TraineeId: {traineeId}", traineeId);
                    return StatusCode(500, new {message = "Simulated Internal Error."});
            }

            var profile = new TraineeProcessingProfileResponse
            {
                TraineeId = traineeId,
                FullName = $"Trainee {traineeId}",
                Email = $"trainee{traineeId}@training.com",
                EnrollmentStatus = "Active"  
            };

            return Ok(profile);

        }
    }
}