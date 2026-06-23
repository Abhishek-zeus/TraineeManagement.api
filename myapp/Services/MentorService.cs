using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.DTOs.Mentors_DTO;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Services
{
    public class MentorService : IMentorService
    {
        private readonly AppDbContext _context;
        public MentorService(AppDbContext context)
        {
            _context = context;
        }

        public RegisterMentorRequest ConvertToDTO(Mentor mentor)
        {
            RegisterMentorRequest response = new()
            {
                FirstName = mentor.FirstName,
                LastName = mentor.LastName,
                Email = mentor.Email,
                Expertise = mentor.Expertise,
                Status = mentor.Status
            };
            return response;
        }

        public Mentor ConvertToModel(RegisterMentorRequest request)
        {
            var mentor = new Mentor()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Expertise = request.Expertise,
                Status = request.Status,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            return mentor;
        }

        public async Task<List<RegisterMentorRequest>> GetMentors()
        {
            var mentors = await _context.Mentors.ToListAsync();
            var dtoPages = mentors.Select(mentor => ConvertToDTO(mentor)).ToList();
            return dtoPages;
        }
        public async Task<RegisterMentorRequest> GetById(int id)
        {
            var mentor = await _context.Mentors.FindAsync(id);
            return  ConvertToDTO(mentor);
        }
        public async Task<RegisterMentorRequest> RegisterMentor(RegisterMentorRequest request)
        {
            var mentor = ConvertToModel(request);
            await _context.Mentors.AddAsync(mentor);
            await _context.SaveChangesAsync();
            return ConvertToDTO(mentor);
        }
        public async Task<RegisterMentorRequest> UpdateMentor(int id, RegisterMentorRequest request)
        {
            var existing = _context.Mentors.FirstOrDefault(m => m.id == id);
            if(existing == null)
            {
                return null;
            }
            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Email = request.Email;
            existing.Expertise = request.Expertise;
            existing.Status = request.Status;

            await _context.SaveChangesAsync();
            return ConvertToDTO(existing);

        }
        public async Task<RegisterMentorRequest> DeleteMentor(int id)
        {
            var existing = _context.Mentors.FirstOrDefault(m => m.id == id);
            if(existing == null)
            {
                return null;
            }
            _context.Mentors.Remove(existing);
            await _context.SaveChangesAsync();

            return ConvertToDTO(existing);
        }
    }
}