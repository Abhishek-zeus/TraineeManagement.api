using Microsoft.AspNetCore.Mvc;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.DTOs.Mentors_DTO;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.myapp.Services;
using TraineeManagement.myapp.Interfaces;

namespace TraineeManagement.myapp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/mentor/[controller]")]
    public class MentorController : ControllerBase
    {
        private readonly IMentorService _service;
        public MentorController(IMentorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<RegisterMentorRequest>>> GetMentors()
        {
            List<RegisterMentorRequest> mentors = await _service.GetMentors();
            if(mentors.Count == 0)
            {
                return NotFound(new {message="No Mentor Found"});
            }
            return Ok(mentors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegisterMentorRequest>> GetById(int id)
        {
            var mentor = await _service.GetById(id);
            if(mentor == null)
            {
                return NotFound(new {message="Mentor not Found"});
            }
            return Ok(mentor);
        }

        [HttpPost]
        public async Task<ActionResult<RegisterMentorRequest>> RegisterMentor(RegisterMentorRequest request)
        {
            var mentor = await _service.RegisterMentor(request);
            return Created(
               mentor.FirstName,
                mentor);
        }

        [HttpPut]
        public async Task<ActionResult<RegisterMentorRequest>> UpdateMentor(int id, RegisterMentorRequest request)
        {
            var mentor = _service.UpdateMentor(id, request);
            if(mentor == null)
            {
                return NotFound(new {message="Mentor not Found"});
            }
            return Ok(mentor);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteMentor(int id)
        {
            var mentor = _service.DeleteMentor(id);
            if(mentor == null)
            {
                return NotFound(new {message="Mentor not Found"});
            }
            return Ok(true);
        }
    }
}