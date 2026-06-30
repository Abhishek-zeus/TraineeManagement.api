using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Controllers
{
    [ApiController]
    [Route("api/processing-jobs")]
    [Authorize(Roles = "Admin,Trainee")] // Restricts the entire controller; Mentors get a 403 Forbidden automatically
    public class ProcessingJobController : ControllerBase
    {
        private readonly IProcessingJobService _jobService;

        public ProcessingJobController(IProcessingJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessingJobResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var job = await _jobService.GetByIdAsync(id, cancellationToken);
            return job == null? NotFound(new {message = $"Processing Job with ID {id} not found"}):Ok(job);
        }
    }
}