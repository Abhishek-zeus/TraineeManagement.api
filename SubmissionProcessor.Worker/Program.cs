using TraineeManagement.SubmissionProcessor.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TraineeManagement.SubmissionProcessor.Worker.Interfaces;
using TraineeManagement.SubmissionProcessor.Worker.Services;
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


/////////////////////////POLLY

builder.Services.AddHttpClient<ITrainingDirectoryClient, TrainingDirectoryClient>(client =>
{
    // Centrally configure BaseAddress
    client.BaseAddress = new Uri(builder.Configuration["TrainingDirectory:BaseUrl"] ?? "http://localhost:5050");
})
.AddStandardResilienceHandler(options =>
{
    // 1. RETRY CONFIGURATION (Fires automatically ONLY on 5xx, 408, 429, or network drops)
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromMilliseconds(300);
    options.Retry.BackoffType = DelayBackoffType.Exponential; // Delays step up dynamically: 300ms -> 600ms -> 1.2s

    // 2. ATTEMPT TIMEOUT (How long an individual request attempt is allowed to hang before failing)
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(2);

    // 3. TOTAL REQUEST TIMEOUT (The hard limit boundary across the original call + all retries combined)
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(15);

    // 4. CIRCUIT BREAKER (Disconnects from the target service if it experiences a critical failure surge)
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
    options.CircuitBreaker.FailureRatio = 0.5;       // Trips open if 50% or more of recent calls fail
    options.CircuitBreaker.MinimumThroughput = 3;    // Requires at least 3 sampling calls within the window
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15); // How long the gate stays locked open
});









//Logger configs to remove unwanted providers and only add a console for logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options => options.IncludeScopes = true);

// 🚀 ADD THIS LINE TO FORCE INFORMATION LOGS TO SHOW:
builder.Logging.SetMinimumLevel(LogLevel.Information);

var host = builder.Build();




// TEMPORARY TEST BLOCK 

// using (var testScope = host.Services.CreateScope())
// {
//     var testClient = testScope.ServiceProvider.GetRequiredService<ITrainingDirectoryClient>();
//     var testLogger = testScope.ServiceProvider.GetRequiredService<ILogger<Program>>();

//     testLogger.LogInformation("=== TEST 1: Normal call ===");
//     var normal = await testClient.GetTraineeProfileAsync(101, "test-correlation-1");
//     testLogger.LogInformation("Result: {FullName}", normal?.FullName ?? "null");

//     testLogger.LogInformation("=== TEST 2: Missing trainee (expect null, not an exception) ===");
//     var missing = await testClient.GetTraineeProfileAsync(9999, "test-correlation-2");
//     testLogger.LogInformation("Result: {Result}", missing == null ? "null (correctly not found)" : "UNEXPECTED VALUE");

//     testLogger.LogInformation("=== TEST 3: Slow trainee (expect retries, then a timeout failure) ===");
//     try
//     {
//         await testClient.GetTraineeProfileAsync(8888, "test-correlation-3");
//     }
//     catch (Exception ex)
//     {
//         testLogger.LogWarning("Failed as expected: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
//     }

//     testLogger.LogInformation("=== TEST 4: Circuit breaker (repeated failures should trip it open) ===");
//     for (int i = 1; i <= 6; i++)
//     {
//         var start = DateTime.UtcNow;
//         try
//         {
//             await testClient.GetTraineeProfileAsync(7777, $"test-correlation-cb-{i}");
//         }
//         catch (Exception ex)
//         {
//             var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;
//             testLogger.LogWarning("Attempt {Attempt} failed after {ElapsedMs:F1}ms: {ExceptionType}", i, elapsed, ex.GetType().Name);
//         }
//         await Task.Delay(300);
//     }
// }








host.Run();
