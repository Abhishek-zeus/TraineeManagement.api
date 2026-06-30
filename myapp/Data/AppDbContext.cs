//Similar to Repo layer in Java (Spring data JPA that automates all the database operations)
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using myapp.Migrations;
using TraineeManagement.myapp.Enums;
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

        public DbSet<Trainee> Trainees { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<LearningTask> Tasks { get; set; }
        public DbSet<TaskAssignment> Assignments { get; set;}
        public DbSet<Submission> Submissions { get; set;}
        public DbSet<Review> Reviews { get; set;}
        public DbSet<SubmissionFile> SubmissionFiles {get; set;}
        public DbSet<ProcessingJob> ProcessingJobs {get; set;}



        // Configuring constraints and indexes for db
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // 1. TRAINEE TABLE 
            modelBuilder.Entity<Trainee>(entity =>
            {
                entity.HasIndex(t => t.Email).IsUnique();
                entity.Property(t => t.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(t => t.LastName).IsRequired().HasMaxLength(50);
                entity.Property(t => t.Email).IsRequired().HasMaxLength(100);
            });


            // 2. USER TABLE 
            modelBuilder.Entity<User>(entity =>
            {
                // Unique constraint on Username/Email
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Role).HasConversion<string>(); 
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            });

            // 3. MENTOR TABLE 
            modelBuilder.Entity<Mentor>(entity =>
            {
                entity.HasIndex(m => m.Email).IsUnique();
                entity.Property(m => m.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(m => m.LastName).IsRequired().HasMaxLength(50);
                entity.Property(m => m.Email).IsRequired().HasMaxLength(100);
            });

            // 4. LEARNING TASK TABLE 
            modelBuilder.Entity<LearningTask>(entity =>
            {
                // Performance index for tasks sorted or filtered by due date
                entity.HasIndex(lt => lt.DueDate);
                entity.HasIndex(lt => lt.Status);

                entity.Property(lt => lt.Title).IsRequired().HasMaxLength(100);
                entity.Property(lt => lt.Description).HasMaxLength(500);
            });

            // Automatically find all DateTime properties in all tables 
            // Loops through every table 
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Loops through evry column and ClrType is DataType
                var properties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

                foreach (var property in properties)
                {
                    // Forces Entity Framework to specify the UTC timezone format on read/write 

                    // Get the underlying type (handles both DateTime and DateTime?)
                    var type = property.ClrType;

                    if (type == typeof(DateTime))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                    }
                    else if (type == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : v.Value.ToUniversalTime()) : null,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
                        ));
                    }

                }
            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.Trainee) //Has only one Trainee 
                .WithMany()             //Trainee can have many Assignments
                .HasForeignKey(t => t.TraineeId); // TraineeId is a foreign key
            }

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.Mentor)
                .WithMany()
                .HasForeignKey(t => t.MentorId);

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.LearningTask)
                .WithMany()
                .HasForeignKey(t => t.LearningTaskId);

            modelBuilder.Entity<ProcessingJob>()
            .Property(p => p.Status)
            .HasConversion<string>();


            //DATA SEEDING for an INITIAL ADMIN
            var adminUser = new User
            {
                id = 1,
                Username = "super_admin",
                Email = "admin@company.com",
                Role = UserRole.Admin
            } ;
            var hasher = new PasswordHasher<User>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser,"AdminSecurePassword");
            //It checks if the database has the entry with id 1, if not then it seeds a adminUser
            modelBuilder.Entity<User>().HasData(adminUser);
        }

    }
}