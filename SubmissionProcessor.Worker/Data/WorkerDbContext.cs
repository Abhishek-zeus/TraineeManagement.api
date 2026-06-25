using Microsoft.EntityFrameworkCore;
using TraineeManagement.SubmissionProcessor.Worker.Models;

namespace TraineeManagement.SubmissionProcessor.Worker.Data
{
    public class WorkerDbContext : DbContext
    {
        public WorkerDbContext(DbContextOptions<WorkerDbContext> options) : base(options)
        { }

        public DbSet<ProcessingJob> ProcessingJobs => Set<ProcessingJob>();
        public DbSet<SubmissionFile> SubmissionFiles => Set<SubmissionFile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TELL EF CORE TO TREAT THE ENUM AS A STRING IN THE DATABASE:
            modelBuilder.Entity<ProcessingJob>()
                .Property(j => j.Status)
                .HasConversion<string>();
        }
    }
}