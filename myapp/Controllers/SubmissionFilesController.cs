using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.Mappers;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Utility;

namespace TraineeManagement.myapp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/submission-files")]
    public class SubmissionFilesController : ControllerBase
    {
        private readonly ISubmissionFileService _submissionFileService;
        private readonly ILogger<SubmissionFilesController> _logger;

        public SubmissionFilesController(ISubmissionFileService submissionFileService, ILogger<SubmissionFilesController> logger)
        {
            _submissionFileService = submissionFileService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CanViewSubmissions")] // Admin, Mentor, Trainee can hit this
        public async Task<ActionResult<SubmissionFileResponse>> GetFileMetadata(int id)
        {
            var metadata = await _submissionFileService.GetFileMetadataAsync(id);
            if(metadata == null)
            {
                return NotFound(new {message = $"File with ID {id} not found."});
            } 
            return Ok(metadata);
        }

        [HttpGet("{id}/download")]
        [Authorize(Policy = "CanViewSubmissions")] // Admin, Mentor, Trainee can hit this
        public async Task<IActionResult> DownloadFile(int id, CancellationToken cancellationToken)
        {
            try
            {
                var (fileStream, originalFileName, contentType) = await _submissionFileService.DownloadFileAsync(id, cancellationToken);
                return File(fileStream, contentType, originalFileName);  //save to 
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new {message = ex.Message});
            }
            catch(FileNotFoundException ex)
            {
                return NotFound(new {message = ex.Message});
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanManageAssignments")] // Only Trainees can delete
        public async Task<IActionResult> DeleteFile(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _submissionFileService.DeleteFileAsync(id, cancellationToken);
                return NoContent();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new {message = ex.Message});
            }
        }

    }
}