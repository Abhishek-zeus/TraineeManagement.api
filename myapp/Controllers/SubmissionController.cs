using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.DTOs.LearningTask_DTO;
using TraineeManagement.myapp.Enums;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Mappers;

namespace TraineeManagement.myapp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/submissions/[controller]")]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _service;
        private readonly ISubmissionFileService _submissionFileService;
        private readonly IProcessingJobService _processingJobService;
        private readonly IMessagePublisher _messagePublisher;
        public SubmissionController(ISubmissionService service, IProcessingJobService processingJobService, ISubmissionFileService submissionFileService, IMessagePublisher messagePublisher)
        {
            _service = service;
            _submissionFileService = submissionFileService;
            _messagePublisher = messagePublisher;
            _processingJobService = processingJobService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateSubmissionRequest>> SubmitTask(CreateSubmissionRequest request)
        {
            var submittedTask = await _service.SubmitTask(request);
            return Created("", submittedTask);
        }

        [HttpGet]
        public async Task<ActionResult<List<CreateSubmissionRequest>>> GetSubmissions()
        {
            List<CreateSubmissionRequest> submittedTask = await _service.GetSubmissions();
            if (submittedTask.Count == 0)
            {
                return NotFound(new { message = "No Submitted Task Found" });
            }
            return Ok(submittedTask);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateSubmissionRequest>> GetById(int id)
        {
            var submittedTask = await _service.GetById(id);
            if (submittedTask == null)
            {
                return NotFound(new { message = "Submitted Task not Found" });
            }
            return Ok(submittedTask);
        }


        [HttpPost("{submissionId}/files")]
        [RequestSizeLimit(10485760)]    //checks combined size of file, headers and form text fields
        [RequestFormLimits(MultipartBodyLengthLimit = 10485760)]    //specific boundary for the file body
        public async Task<ActionResult<SubmissionFileResponse>> UploadFile(int submissionId, IFormFile file, CancellationToken cancellationToken)
        {
            //Get user id from jwt and User is the builtin property of ASP.NET core 
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //!int.TryParse(userIdClaim, out int uploadedByUserId) is used to convert userIDClaim to valid integer and if it succeeds it will assign that value to uploadedByUserID
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int uploadedByUserId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            try
            {
                var response = await _submissionFileService.UploadFileAsync(submissionId, file, uploadedByUserId, cancellationToken);

                //Take or Generate a unique Correlation Tracking string
                var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();

                //Assemble our message package
                var queueMessage = new SubmissionProcessingRequest
                {
                    CorrelationId = correlationId,
                    SubmissionId = submissionId,
                    FileId = response.Id
                };


                var job = await _processingJobService.CreateQueuedJobAsync(queueMessage.MessageId, correlationId, submissionId, response.Id, cancellationToken);



                //send directly to the queue
                try
                {
                    _messagePublisher.PublishSubmissionTask(queueMessage);
                }
                catch (Exception publishEx)
                {
                    //If queueing fails, immediately mark the database job row as "Failed"
                    await _processingJobService.UpdateJobToFailedAsync(job.Id, $"Queue publishing failed: {publishEx.Message}", cancellationToken);
                    return StatusCode(500, new { message = "File uploaded, but failed to queue the background processing job.", error = publishEx.Message });
                }

                // return 202 Accepted
                return Accepted(new
                {
                    message = "File Uploaded successfully. Processing job queued",
                    fileId = response.Id,
                    correlationId = correlationId,
                    processingJobId = job.Id
                });

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{id}/summary")]
        public async Task<ActionResult<SubmissionSummaryResponse>> GetSummary(int id, CancellationToken cancellationToken)
        {
            var summary = await _service.GetSummaryAsync(id, cancellationToken);
            return summary == null ? NotFound(new { message = $"Submission with ID {id} not found" }) : Ok(summary);
        }
    }
}