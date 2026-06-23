using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Services;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.DTOs.LearningTask_DTO;
using TraineeManagement.myapp.Enums;
using TraineeManagement.myapp.Interfaces;

namespace TraineeManagement.myapp.Controllers 
{
    [Authorize]
    [ApiController]
    [Route("api/task-assignments/[controller]")]
    public class TaskAssignmentController : ControllerBase
    {
        private readonly ITaskAssignmentService _service;
        public TaskAssignmentController(ITaskAssignmentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<CreateTaskAssignmentRequest>> AssignTask(CreateTaskAssignmentRequest request)
        {
            var assignedTask = await _service.AssignTask(request);
            if(assignedTask == null)
            {
                return NotFound("Trainee/ Mentor / Learning task not found");
            }
            return Created("",assignedTask);
        }

        [HttpGet]
        public async Task<ActionResult<List<CreateTaskAssignmentRequest>>> GetAssignments()
        {
            List<CreateTaskAssignmentRequest> assignedTasks = await _service.GetAssignments();
            if(assignedTasks.Count == 0)
            {
                return NotFound(new {message="No assignedTasks Found"});
            }
            return Ok(assignedTasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateTaskAssignmentRequest>> GetById(int id, CancellationToken cancellationToken)
        {
            var assignedTask = await _service.GetById(id, cancellationToken);
            if(assignedTask == null)
            {
                return NotFound(new {message="AssignedTask not Found"});
            }
            return Ok(assignedTask);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<CreateTaskAssignmentRequest>> UpdateAssignedTask(int id, AssignmentStatus status, CancellationToken cancellationToken)
        {
            var assignedTask = _service.UpdateTask(id, status, cancellationToken);
            if(assignedTask == null)
            {
                return NotFound(new {message="Assigned Task not Found"});
            }
            return Ok(assignedTask);
        }

    }
}