using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Services;

namespace TraineeManagement.myapp.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController : ControllerBase
    {
        // for dependency injection
        private readonly ITraineeService service;
        public TraineeController(ITraineeService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(String? search){
            if(!String.IsNullOrEmpty(search))
            {
                List<Trainee> trainees = await service.Search(search);
                if(trainees.Count == 0){
                    return NotFound();
                }
                return Ok(trainees);
            }
            return Ok(await service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            Trainee trainee = await service.GetById(id);
            if(trainee == null)
            {
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
                return NotFound();
            }
            
            return Ok(await service.Update(id,trainee));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id){
            Trainee? trainee = await service.Delete(id);
            if(trainee == null){
                return NotFound();
            }
            return NoContent();
        }
    }
}