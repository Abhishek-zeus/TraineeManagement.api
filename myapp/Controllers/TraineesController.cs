using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;



namespace TraineeManagement.myapp.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController : ControllerBase
    {
        // for dependency injection
        private readonly ITraineeService _service;
        private readonly ILogger<TraineeController> _logger;
        public TraineeController(ITraineeService service, ILogger<TraineeController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<TraineeDTO>>> GetTrainees(int? pageNumber, int? pageSize, string? search, string? status){
            if(pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Bad Request, PageNumber and PageSize should be positive");
            }
            var allResult = await _service.GetTrainees(pageNumber,pageSize,search,status);
            return Ok(allResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Trainee>> GetById(int id, CancellationToken cancellationToken)
        {
            Trainee trainee = await _service.GetById(id, cancellationToken);
            if(trainee == null)
            {
                _logger.LogInformation("Trainee not found {id}",id);
                return Ok(new {message="No Trainee Found"});
            }
            return Ok(trainee);
        }

      
        [HttpPost]
        public async Task<ActionResult<Trainee>> CreateTrainee(TraineeDTO request)
        {
            
            var result = await _service.CreateTrainee(request);
            if(result == null)
            {
                return Conflict(new {Message= "Trainee with specific Email already exists"});
            }

            return CreatedAtAction(
                nameof(GetById),
                new {id = result.id},
                result);
        }

       
        [HttpPut("{id}")]
        public async Task<ActionResult<Trainee>> UpdateTrainee(int id, TraineeDTO request, CancellationToken cancellationToken)
        {
            var updatedTrainee = await _service.UpdateTrainee(id,request, cancellationToken);
            if(updatedTrainee == null){
                _logger.LogWarning("Trainee not found {id}",id);
                return NotFound(new {message="No Trainee Found"});
            }
            
            return Ok(updatedTrainee);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteTrainee(int id, CancellationToken cancellationToken)
        {
            Trainee? trainee = await _service.DeleteTrainee(id, cancellationToken);
            if(trainee == null){
                _logger.LogWarning("Trainee not found {id}",id);
                return NotFound(new {message="No Trainee Found"});
            }
            return Ok(true);
        }
    }
}