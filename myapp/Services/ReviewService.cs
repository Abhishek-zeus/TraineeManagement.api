using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.Enums;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Mappers;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Interfaces;

namespace TraineeManagement.myapp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        public ReviewService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CreateReviewRequest> ReviewTask(CreateReviewRequest request)
        {
            var reviewedTask = ReviewMapper.ToEntity(request);
            reviewedTask.ReviewedDate = DateTime.UtcNow;
            await _context.Reviews.AddAsync(reviewedTask);
            await _context.SaveChangesAsync();
            return ReviewMapper.ToDTO(reviewedTask);
        }


        public async Task<List<CreateReviewRequest>> GetReviews()
        {
            var reviewedTasks = await _context.Reviews.ToListAsync();
            var dtoPages = reviewedTasks.Select(reviewedTask => ReviewMapper.ToDTO(reviewedTask)).ToList();
            return dtoPages;
        }
        public async Task<CreateReviewRequest> GetById(int Id)
        {
            var reviewedTask = await _context.Reviews.FindAsync(Id);
            return ReviewMapper.ToDTO(reviewedTask);
        }

    }
}