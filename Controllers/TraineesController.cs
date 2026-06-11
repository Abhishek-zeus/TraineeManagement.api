using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.myapp.Services;

/*
TODO:
1) Update Method Names to appropriate names like GetTrainees instead of GetAll, CreateTrainee instead of Create, UpdateTrainee instead of Update and DeleteTrainee instead of Delete
2) Replace return type with appropriate response type instead of IActionResult
3) Separate out the global variable with _ like _service and _logger
*/

namespace TraineeManagement.myapp.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController : ControllerBase
    {
        // for dependency injection
        private readonly ITraineeService service;
        private readonly ILogger<TraineeController> logger;
        public TraineeController(ITraineeService service, ILogger<TraineeController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        /*
        TODO: 
        1) Replace current logic with search and filter logic in service layer
        2) Remove Not Found always return 200 if api is successfully executed even if no data is found, return empty list with 200 status code
        3) Remove warning logs for not found cases, as it is not an error scenario, it is a valid scenario that no data is found for given search or filter criteria
        4) Add validation check for pageNumber and pageSize, if they are less than or equal to 0, return bad request with appropriate message and pass default value here
        5) instead of String use primitive type string 

        */
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

        /*
        TODO:
        1) Add validation for request body, if any required field is missing or invalid, return bad request with appropriate message
        2) Create mapper file to map the object but in this case pass the request object to service layer and do the mapping in service layer, it will make controller code cleaner and also separation of concern will be maintained

        */
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

        /*
        TODO:
        1) Add validation for request body, if any required field is missing or invalid, return bad request with appropriate message
        2) Duplicate code for update is called 
        3) Update Mapper

        */
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

        /*
        TODO:
        1) After delete return boolean not the deleted entity

        */
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