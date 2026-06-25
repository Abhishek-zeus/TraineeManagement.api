using TraineeManagement.SubmissionProcessor.Worker;
using Microsoft.EntityFrameworkCore;

using TraineeManagement.SubmissionProcessor.Worker.Data;
using TraineeManagement.SubmissionProcessor.Worker.Interfaces;
using TraineeManagement.SubmissionProcessor.Worker.Services;
using TraineeManagement.SubmissionProcessor.Worker.Utility;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorage")
);

builder.Services.Configure<ProcessingSettings>(
    builder.Configuration.GetSection("ProcessingSettings")
);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<WorkerDbContext>(options =>
    options.UseMySQL(connectionString));


builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();


//Logger configs to remove unwanted providers and only add a console for logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// 🚀 ADD THIS LINE TO FORCE INFORMATION LOGS TO SHOW:
builder.Logging.SetMinimumLevel(LogLevel.Information);

var host = builder.Build();
host.Run();
