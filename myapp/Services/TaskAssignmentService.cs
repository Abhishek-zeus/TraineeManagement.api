using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.Enums;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Mappers;
using TraineeManagement.myapp.Utility;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Interfaces;

namespace TraineeManagement.myapp.Services
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly AppDbContext _context;
        private readonly ICacheService _cache;
        private readonly CacheSettings _cacheSettings;
        public TaskAssignmentService(AppDbContext context, ICacheService cache,  IOptions<CacheSettings> options)
        {
            _context = context;
            _cacheSettings = options.Value;
            _cache = cache;

        }
        public async Task<CreateTaskAssignmentRequest> AssignTask(CreateTaskAssignmentRequest request)
        {
            var trainee = await _context.Trainees.FindAsync(request.TraineeId);
            if (trainee == null)
            {
                return null;
            }

            var mentor = await _context.Mentors.FindAsync(request.MentorId);
            if (mentor == null)
            {
                return null;
            }

            var learningTask = await _context.Tasks.FindAsync(request.LearningTaskId);
            if (learningTask == null)
            {
                return null;
            }
            var assignedTask = TaskAssignmentMapper.ToEntity(request);
            assignedTask.AssignedDate = DateTime.UtcNow;
            _context.Assignments.Add(assignedTask);
            await _context.SaveChangesAsync();
            return TaskAssignmentMapper.ToDTO(assignedTask);
        }


        public async Task<List<CreateTaskAssignmentRequest>> GetAssignments()
        {
            var assignedTasks = await _context.Assignments.ToListAsync();
            var dtoPages = assignedTasks.Select(assignedTask => TaskAssignmentMapper.ToDTO(assignedTask)).ToList();
            return dtoPages;
        }
        public async Task<CreateTaskAssignmentRequest> GetById(int Id, CancellationToken cancellationToken)
        {
            var cacheKey = $"task-assignment:{Id}";
            //Step 1 : Try Cache first
            var cached = await _cache.GetAsync<CreateTaskAssignmentRequest>(cacheKey, cancellationToken);
            if(cached != null)
            {
                return cached; //Cache HIT
            }
            //Step 2 : Cache MISS - go to DB
            var assignedTask = await _context.Assignments.FindAsync(Id);
            if(assignedTask == null)
            {
                return null; // Dont cache not found results
            }
            //Step 3 : Populate cache for next time
            var ttl = TimeSpan.FromSeconds(_cacheSettings.TaskAssignmentTtlSeconds);
            await _cache.SetAsync(cacheKey, assignedTask, ttl, cancellationToken);
            return TaskAssignmentMapper.ToDTO(assignedTask);
        }

        public async Task<CreateTaskAssignmentRequest> UpdateTask(int Id, AssignmentStatus status, CancellationToken cancellationToken)
        {
            var existing = _context.Assignments.FirstOrDefault(t => t.Id == Id);
            if (existing == null)
            {
                return null;
            }
            existing.Status = status;

            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"task-assignment:{Id}",cancellationToken);
            return TaskAssignmentMapper.ToDTO(existing);

        }

    }
}