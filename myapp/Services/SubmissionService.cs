using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.Enums;
using Microsoft.Extensions.Options;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Mappers;
using TraineeManagement.myapp.Utility;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Interfaces;

namespace TraineeManagement.myapp.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly AppDbContext _context;
        private readonly ICacheService _cache;
        private readonly CacheSettings _cacheSettings;
        private readonly ILogger<SubmissionService> _logger;


        public SubmissionService(AppDbContext context, ICacheService cache, ILogger<SubmissionService> logger, IOptions<CacheSettings> options)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
            _cacheSettings =  options.Value;
        }
        public async Task<CreateSubmissionRequest> SubmitTask(CreateSubmissionRequest request)
        {
            var submittedTask = SubmissionMapper.ToEntity(request);
            submittedTask.SubmittedDate = DateTime.UtcNow;
            await _context.Submissions.AddAsync(submittedTask);
            await _context.SaveChangesAsync();
            return SubmissionMapper.ToDTO(submittedTask);
        }


        public async Task<List<CreateSubmissionRequest>> GetSubmissions()
        {
            var submittedTasks = await _context.Submissions.ToListAsync();
            var dtoPages = submittedTasks.Select(submittedTask => SubmissionMapper.ToDTO(submittedTask)).ToList();
            return dtoPages;
        }
        public async Task<CreateSubmissionRequest> GetById(int Id)
        {
            var submittedTask = await _context.Submissions.FindAsync(Id);
            return SubmissionMapper.ToDTO(submittedTask);
        }


        public async Task<SubmissionSummaryResponse?> GetSummaryAsync(int submissionId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"submission-summary:{submissionId}";
            var cached = await _cache.GetAsync<SubmissionSummaryResponse>(cacheKey, cancellationToken);
            if(cached != null)
            {
                return cached;
            }
            var submission = await _context.Submissions
                            .Where(s => s.Id == submissionId)
                            .FirstOrDefaultAsync(cancellationToken);
            if(submission == null)
            {
                return null;
            }
            var summary = SubmissionSummaryMapper.ToDTO(submission);

            var ttl = TimeSpan.FromSeconds(_cacheSettings.SubmissionSummaryCacheTtlSeconds);
            await _cache.SetAsync(cacheKey, summary, ttl, cancellationToken);
            return summary;          
        }

    }
}