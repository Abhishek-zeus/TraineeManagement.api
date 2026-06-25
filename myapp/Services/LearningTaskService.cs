using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.DTOs.LearningTask_DTO;
using TraineeManagement.myapp.Mappers;
using TraineeManagement.myapp.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace TraineeManagement.myapp.Services
{
    public class LearningTaskService : ILearningTaskService
    {
        private readonly AppDbContext _context;
        public LearningTaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CreateTaskRequest>> GetLearningTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            var dtoPages = tasks.Select(task => LearningTaskMapper.ConvertToDTO(task)).ToList();
            return dtoPages;
        }
        public async Task<CreateTaskRequest> GetById(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            return  LearningTaskMapper.ConvertToDTO(task);
        }
        public async Task<CreateTaskRequest> RegisterTask(CreateTaskRequest request)
        {
            var task = LearningTaskMapper.ConvertToModel(request);
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return LearningTaskMapper.ConvertToDTO(task);
        }
        public async Task<CreateTaskRequest> UpdateTask(int id, CreateTaskRequest request)
        {
            var existing = _context.Tasks.FirstOrDefault(t => t.id == id);
            if(existing == null)
            {
                return null;
            }
            existing.Title = request.Title;
            existing.Description = request.Description;
            existing.ExpectedTechStack = request.ExpectedTechStack;
            existing.DueDate = request.DueDate;
            existing.Status = request.Status;

            await _context.SaveChangesAsync();
            return LearningTaskMapper.ConvertToDTO(existing);

        }
        public async Task<CreateTaskRequest> DeleteTask(int id)
        {
            var existing = _context.Tasks.FirstOrDefault(t => t.id == id);
            if(existing == null)
            {
                return null;
            }
            _context.Tasks.Remove(existing);
            await _context.SaveChangesAsync();

            return LearningTaskMapper.ConvertToDTO(existing);
        }
    }
}