using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Data;
using TraineeManagement.myapp.DTOs;


namespace TraineeManagement.myapp.Services
{
    public class TraineeService : ITraineeService
    {
        //Inject DbContext and Logger
        private readonly AppDbContext context;
        private readonly ILogger<TraineeService> logger;
        public TraineeService(AppDbContext _context, ILogger<TraineeService> logger)
        {
            context = _context;
            this.logger = logger;
        }

        private static int nextId = 1;

        //added async and await that return Task<>

        /*
        TODO: 
        1) Replace CreateTraineeRequest with some dto it is confusing to use request dto in response
        2) Use generatic paged response

        */
        public async Task<PagedResponse> GetAll(int? pageNumber, int? pageSize)
        { 

            int totalRecords = await context.Trainees.CountAsync();
            List<CreateTraineeRequest> data;
            //Do pagination only if pageNumber & pagesize is provided
            int validPageNumber = pageNumber ?? 1;
            int validPageSize = pageSize ?? 10;
            if(pageNumber.HasValue || pageSize.HasValue){
                // if any any value is missing

                data = await context.Trainees.OrderBy(p => p.id).
                // TODO: Extract similar logic in mapper
                Select(t => new CreateTraineeRequest
                {
                    firstName = t.FirstName,
                    lastName = t.LastName,
                    email = t.Email,
                    techStack = t.TechStack,
                    status = t.Status
                }).
                Skip((validPageNumber -1) * validPageSize).Take(validPageSize).ToListAsync();
            }
            // else return all records
            else{
                data = await context.Trainees.Select
                (t => new CreateTraineeRequest 
                {
                    firstName = t.FirstName,
                    lastName = t.LastName,
                    email = t.Email,
                    techStack = t.TechStack,
                    status = t.Status
                }).ToListAsync();
            }
            return new PagedResponse(validPageNumber,validPageSize,totalRecords,data);
            
        }

        public async Task<Trainee> GetById(int id)
        {
            return await context.Trainees.FindAsync(id);
        }

        public async Task<Trainee> Create(Trainee trainee)
        {
            // TODO: Add auto increment at database level and remove this logic from heres
            trainee.id = nextId++;
            // TODO: Use UTC time and store in same format across the application
            trainee.CreatedDate = DateTime.Now;
            trainee.UpdatedDate = DateTime.Now;

            // TODO: Add validation for duplicate email and other fields as necessary and return appropriate response.
            await context.Trainees.AddAsync(trainee);
            await context.SaveChangesAsync();
            logger.LogInformation("Trainee Created with Id {id}",trainee.id);
            return trainee;
        }


        public async Task<Trainee> Update(int id, Trainee trainee)
        {
            var existing = await context.Trainees.FindAsync(id);
            if(existing == null){
                return null;
            }
            existing.FirstName = trainee.FirstName;
            existing.LastName = trainee.LastName;
            existing.Email = trainee.Email;
            existing.TechStack = trainee.TechStack;
            existing.UpdatedDate = DateTime.Now;

            await context.SaveChangesAsync();
            logger.LogInformation("Trainee {id} updated successfully");
            return existing;
        }


        public async Task<Trainee> Delete(int id)
        {
            var trainee = await context.Trainees.FindAsync(id);
            if(trainee == null){
                return null;
            }

            //only remove does not use await async
            context.Trainees.Remove(trainee);
            await context.SaveChangesAsync();

            logger.LogInformation("Trainee deleted {id}",id);
            return trainee;
        }


        public async Task<PagedResponse> Search(int? pageNumber, int? pageSize, String? search, String? status)
        {
            var query = context.Trainees.AsQueryable();

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
            List<CreateTraineeRequest> data;

            // if any any value is missing
            int validPageNumber = pageNumber ?? 1;
            int validPageSize = pageSize ?? 10;

            if(pageNumber.HasValue || pageSize.HasValue){

               
                data = await query.OrderBy(p => p.id).Select
                (t => new CreateTraineeRequest 
                {
                    firstName = t.FirstName,
                    lastName = t.LastName,
                    email = t.Email,
                    techStack = t.TechStack,
                    status = t.Status
                }).Skip((validPageNumber -1) * validPageSize).Take(validPageSize).ToListAsync();
            }
            else
            {

                data = await query.OrderBy(p => p.id).Select(t => new CreateTraineeRequest 
                {
                    firstName = t.FirstName,
                    lastName = t.LastName,
                    email = t.Email,
                    techStack = t.TechStack,
                    status = t.Status
                }).ToListAsync();
                
            }

            return new PagedResponse(validPageNumber,validPageSize,totalRecords,data);
        }
    }
}