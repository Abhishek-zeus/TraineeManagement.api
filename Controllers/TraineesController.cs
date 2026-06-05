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
        public IActionResult GetAll(){
            return Ok(service.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Trainee trainee = service.GetById(id);
            if(trainee == null)
            {
                return NotFound();
            }
            return Ok(trainee);
        }

        [HttpPost]
        public IActionResult Create(CreateTraineeRequest request)
        {
            var trainee = new Trainee
            {
                FirstName = request.firstName,
                LastName = request.lastName,
                Email = request.email,
                TechStack = request.techStack,
                Status = request.status
            };

            service.Create(trainee);

            return CreatedAtAction(
                nameof(GetById),
                new {id = trainee.id},
                trainee);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateTraineeRequest request)
        {
            var trainee = new Trainee
            {
                FirstName = request.firstName,
                LastName = request.lastName,
                Email = request.email,
                TechStack = request.techStack,
                Status = request.status
            };
            
            if(service.Update(id,trainee) == null){
                return NotFound();
            }
            
            return Ok(service.Update(id,trainee));
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id){
            Trainee trainee = service.Delete(id);
            if(trainee == null){
                return NotFound();
            }
            return NoContent();
        }
    }
}