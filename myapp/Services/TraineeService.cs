using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Mappers;
using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.Utility;
using myapp.Migrations;
using Microsoft.Extensions.Options;


namespace TraineeManagement.myapp.Services
{
    public class TraineeService : ITraineeService
    {
        //Inject Db_context and Logger
        private readonly AppDbContext _context;
        private readonly ILogger<TraineeService> _logger;
        private readonly ICacheService _cache;
        private readonly CacheSettings _cacheSettings;
        public TraineeService(AppDbContext context, ILogger<TraineeService> logger, ICacheService cache, IOptions<CacheSettings> options)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
            _cacheSettings = options.Value;
        }

        public async Task<PagedResponse<TraineeDTO>> GetTrainees(int? pageNumber, int? pageSize, string? search, string? status)
        { 

            int totalRecords = await _context.Trainees.CountAsync();
            if(!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(status))
            {
                return await Search(pageNumber,pageSize,search,status);
            }
            List<TraineeDTO> data;
            //Do pagination only if pageNumber & pagesize is provided
            int validPageNumber = pageNumber ?? 1;
            int validPageSize = pageSize ?? 10;

            data = await _context.Trainees.OrderBy(p => p.id).
                Select(t => TraineeMapper.ToDTO(t)).
                Skip((validPageNumber -1) * validPageSize).Take(validPageSize).ToListAsync();
            
            return new PagedResponse<TraineeDTO>(validPageNumber,validPageSize,totalRecords,data);
            
        }

        public async Task<Trainee> GetById(int id, CancellationToken cancellationToken)
        {
            var cacheKey = $"trainee:{id}";
            //Step 1 : Try Cache first
            var cached = await _cache.GetAsync<Trainee>(cacheKey, cancellationToken);
            if(cached != null)
            {
                return cached; //Cache HIT
            }

            //Step 2 : Cache MISS - go to DB
            var trainee = await _context.Trainees.FindAsync(id);
            if(trainee == null)
            {
                return null; // Dont cache not found results
            }

            //Step 3 : Populate cache for next time
            var ttl = TimeSpan.FromSeconds(_cacheSettings.TraineeCacheTtlSeconds);
            await _cache.SetAsync(cacheKey, trainee, ttl, cancellationToken);
            return trainee;
        }

        public async Task<Trainee> CreateTrainee(TraineeDTO request)
        {
            var trainee = TraineeMapper.ToEntity(request);
            trainee.CreatedDate = DateTime.UtcNow;
            trainee.UpdatedDate = DateTime.UtcNow;

            // TODO: Add validation for duplicate email and other fields as necessary and return appropriate response.
            var existing = _context.Trainees.FirstOrDefault(t => t.Email == trainee.Email);
            if(existing != null)
            {
                return null;
            }
            await _context.Trainees.AddAsync(trainee);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Trainee Created with Id {id}",trainee.id);
            return trainee;
        }


        public async Task<Trainee> UpdateTrainee(int id, TraineeDTO request, CancellationToken cancellationToken)
        {
            var trainee = TraineeMapper.ToEntity(request);
            var existing = await _context.Trainees.FindAsync(id);
            if(existing == null){
                return null;
            }
            existing.FirstName = trainee.FirstName;
            existing.LastName = trainee.LastName;
            existing.Email = trainee.Email;
            existing.TechStack = trainee.TechStack;
            existing.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            await _cache.RemoveAsync($"trainee:{id}",cancellationToken);
            _logger.LogInformation("Trainee {id} updated successfully");
            return existing;
        }


        public async Task<Trainee> DeleteTrainee(int id, CancellationToken cancellationToken)
        {
            var trainee = await _context.Trainees.FindAsync(id);
            if(trainee == null){
                return null;
            }

            //only remove does not use await async
            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"trainee:{id}",cancellationToken);
            _logger.LogInformation("Trainee deleted {id}",id);
            return trainee;
        }


        public async Task<PagedResponse<TraineeDTO>> Search(int? pageNumber, int? pageSize, String? search, String? status)
        {
            var query = _context.Trainees.AsQueryable();

            if(!String.IsNullOrEmpty(search))
            {
                query = query.Where(
                    t =>
                    (t.FirstName!.Contains(search) ||
                    t.LastName!.Contains(search) ||
                    t.Email.Contains(search) ||
                    t.TechStack.Contains(search)
                    )
                );
            }

            if(!String.IsNullOrEmpty(status))
            {
                query = query.Where(
                    t =>
                    (t.Status!.Contains(status))
                );
            }

            int totalRecords = await query.CountAsync();
            List<TraineeDTO> data;

            // if any any value is missing
            int validPageNumber = pageNumber ?? 1;
            int validPageSize = pageSize ?? 10;

                data = await query.OrderBy(p => p.id).Select
                (t => TraineeMapper.ToDTO(t)).Skip((validPageNumber -1) * validPageSize).Take(validPageSize).ToListAsync();
            
            return new PagedResponse<TraineeDTO>(validPageNumber,validPageSize,totalRecords,data);
        }
    }
}