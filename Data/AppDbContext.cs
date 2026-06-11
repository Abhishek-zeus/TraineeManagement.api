//Similar to Repo layer in Java (Spring data JPA that automates all the database operations)
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Models;

/*
TODO:

1) Add indexing and constraints to the database tables if not already done

*/
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