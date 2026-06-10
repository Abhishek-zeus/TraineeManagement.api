//Similar to Repo layer in Java (Spring data JPA that automates all the database operations)
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext()
        {

        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Trainee> Trainees{get; set;}

        public DbSet<User> Users{get; set;}
    }
}