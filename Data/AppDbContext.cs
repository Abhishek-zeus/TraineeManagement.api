using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Trainee> Trainees{get; set;}
    }
}