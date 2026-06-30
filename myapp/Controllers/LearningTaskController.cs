using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.DTOs.LearningTask_DTO;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Enums;
using TraineeManagement.myapp.Services;

namespace TraineeManagement.myapp.Controllers
{
    [ApiController]
    [Route("api/learningTask/[controller]")]
    public class LearningTaskController : ControllerBase
    {
        private readonly ILearningTaskService _service;
        public LearningTaskController(ILearningTaskService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize] //Everyone logged in can view
        public async Task<ActionResult<List<CreateTaskRequest>>> GetLearningTasks()
        {
            List<CreateTaskRequest> tasks = await _service.GetLearningTasks();
            if(tasks.Count == 0)
            {
                return Ok(new {message="No Task Found"});
            }
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        [Authorize] //Everyone logged in can view
        public async Task<ActionResult<CreateTaskRequest>> GetById(int id)
        {
            var task = await _service.GetById(id);
            if(task == null)
            {
                return Ok(new {message="No Task Found"});
            }
            return Ok(task);
        }

        [HttpPost]
        [Authorize(Roles = "Mentor")] // only Mentors can access
        public async Task<ActionResult<CreateTaskRequest>> RegisterTask(CreateTaskRequest request)
        {
            var task = await _service.RegisterTask(request);
            return Created(
               task.Title,
                task);
        }

        [HttpPut]
        [Authorize(Roles = "Mentor")] // only Mentors can access
        public async Task<ActionResult<CreateTaskRequest>> UpdateTask(int id, CreateTaskRequest request)
        {
            var task = _service.UpdateTask(id, request);
            if(task == null)
            {
                return Ok(new {message="No Task Found"});
            }
            return Ok(task);
        }


        //Returning IActionResult explicitly for DeletTask as it also returns NoContent()
        [HttpDelete("{id}")]
        [Authorize(Roles = "Mentor")] // only Mentors can access
        public async Task<ActionResult<bool>> DeleteTask(int id)
        {
            var task = _service.DeleteTask(id);
            if(task == null)
            {
                return NotFound(new {message="Task not Found"});
            }
            return Ok(true);
        }
    }
}