using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Data;

namespace TraineeManagement.myapp.Services
{
    public class TraineeService : ITraineeService
    {
        //Inject DbContext
        private readonly AppDbContext context;
        public TraineeService(AppDbContext _context)
        {
            context = _context;
        }

        private static int nextId = 1;

        //added async and await that return Task<>

        public async Task<List<Trainee>> GetAll()
        {
            return await context.Trainees.ToListAsync();
        }

        public async Task<Trainee> GetById(int id)
        {
            return await context.Trainees.FindAsync(id);
        }

        public async Task<Trainee> Create(Trainee trainee)
        {
            trainee.id = nextId++;
            trainee.CreatedDate = DateTime.Now;
            trainee.UpdatedDate = DateTime.Now;

            await context.Trainees.AddAsync(trainee);
            await context.SaveChangesAsync();

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

            return trainee;
        }


        public async Task<List<Trainee>> Search(String search)
        {
            return await context.Trainees.Where(
                t =>
                t.FirstName!.Contains(search) ||
                t.LastName!.Contains(search) ||
                t.Email!.Contains(search) ||
                t.TechStack.Contains(search)
            ).ToListAsync();
        }
    }
}