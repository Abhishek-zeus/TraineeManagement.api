using SubmissionProcessor.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

//Logger configs to remove unwanted providers and only add a console for logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// 🚀 ADD THIS LINE TO FORCE INFORMATION LOGS TO SHOW:
builder.Logging.SetMinimumLevel(LogLevel.Information);

var host = builder.Build();
host.Run();
