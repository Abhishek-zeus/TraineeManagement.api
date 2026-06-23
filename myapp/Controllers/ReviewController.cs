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
    [Route("api/reviews/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;
        public ReviewController(IReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<CreateReviewRequest>> ReviewTask(CreateReviewRequest request)
        {
            var reviewedTask = await _service.ReviewTask(request);
            return Created();
        }

        [HttpGet]
        public async Task<ActionResult<List<CreateReviewRequest>>> GetReviews()
        {
            List<CreateReviewRequest> reviewedTask = await _service.GetReviews();
            if(reviewedTask.Count == 0)
            {
                return NotFound(new {message="No reviewed Task Found"});
            }
            return Ok(reviewedTask);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateReviewRequest>> GetById(int id)
        {
            var reviewedTask = await _service.GetById(id);
            if(reviewedTask == null)
            {
                return NotFound(new {message="Reviewed Task not Found"});
            }
            return Ok(reviewedTask);
        }
    }
}