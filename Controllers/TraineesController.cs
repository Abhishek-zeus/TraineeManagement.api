using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.myapp.Services;

namespace TraineeManagement.myapp.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController : ControllerBase
    {
        // for dependency injection
        private readonly ITraineeService service;
        private readonly ILogger logger;
        public TraineeController(ITraineeService service, ILogger logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int? pageNumber, int? pageSize, String? search, String? status){
            if(!String.IsNullOrEmpty(search) || !String.IsNullOrEmpty(status))
            {
                var pagedResult = await service.Search(pageNumber,pageSize, search, status);
                if(pagedResult.data.Count == 0){
                    logger.LogWarning("No Trainee found");
                    return NotFound();
                }
                return Ok(pagedResult);
            }
            var allResult = await service.GetAll(pageNumber,pageSize);
            return Ok(allResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            Trainee trainee = await service.GetById(id);
            if(trainee == null)
            {
                logger.LogWarning("Trainee not found {id}",id);
                return NotFound();
            }
            return Ok(trainee);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTraineeRequest request)
        {
            var trainee = new Trainee
            {
                FirstName = request.firstName,
                LastName = request.lastName,
                Email = request.email,
                TechStack = request.techStack,
                Status = request.status
            };

            await service.Create(trainee);

            return CreatedAtAction(
                nameof(GetById),
                new {id = trainee.id},
                trainee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTraineeRequest request)
        {
            var trainee = new Trainee
            {
                FirstName = request.firstName,
                LastName = request.lastName,
                Email = request.email,
                TechStack = request.techStack,
                Status = request.status
            };
            
            if(await service.Update(id,trainee) == null){
                logger.LogWarning("Trainee not found {id}",id);
                return NotFound();
            }
            
            return Ok(await service.Update(id,trainee));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id){
            Trainee? trainee = await service.Delete(id);
            if(trainee == null){
                logger.LogWarning("Trainee not found {id}",id);
                return NotFound();
            }
            return NoContent();
        }
    }
}