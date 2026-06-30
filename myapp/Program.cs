using TraineeManagement.myapp.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TraineeManagement.myapp.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TraineeManagement.myapp.Utility;
using DotNetEnv;
using TraineeManagement.myapp.Middleware;
using TraineeManagement.myapp.HealthChecks;
using TraineeManagement.myapp.Controllers;
using TraineeManagement.myapp.Enums;

Env.Load(); // Loads the .env file into environment variables for Jwt
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// Simple NSwag
// builder.Services.AddOpenApiDocument();

//Dependency Injection
// builder.Services.AddSingleton<ITraineeService,TraineeService>(); 

//EFCORE implementation
// builder.Services.AddDbContext<AppDbContext>(
//     options => options.UseInMemoryDatabase("TraineeManagementDb")
// );


builder.Services.AddHttpClient();   //for TrainingDirectoryHealthCheck's IHttpClientFactory

// ----- HEALTH CHECKS -----
builder.Services.AddHealthChecks()
    .AddCheck<MySqlHealthCheck>("mysql", tags: new[] {"ready"})
    .AddCheck<RedisHealthCheck>("redis", tags: new[] {"ready"})
    .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: new[] {"ready"})
    .AddCheck<TrainingDirectoryHealthCheck>("training-directory", tags: new[] {"ready"});






////MYSQL implementation
//connection mentioned in appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//connect to that connection
builder.Services.AddDbContext<AppDbContext>(
    options =>
    options.UseMySQL(connectionString)
);

// Bind FileStorage configuration section to FileStorageSettings class
builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection("FileStorage"));
// Bind CacheSettings configuration section to CacheSettings class
builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
// Bind RabbitMQ configuration section to RabbitMqSettings class
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));



//Dependency Injection
builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<TraineeManagement.myapp.Utility.TokenGeneration>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<ISubmissionFileService, SubmissionFileService>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IProcessingJobService,ProcessingJobService>();


//Enable Multipart form data needed for file uploads
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760;
});

//It registers IDistributedCache in the DI container, backed by Redis.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis"); 
    options.InstanceName ="TraineeManagement_"; // prefixes all keys, avoids collisions if Redis is shared
});



//Register Authentication using JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            RoleClaimType = System.Security.Claims.ClaimTypes.Role, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    }
);

//Adding Authorization Policies
builder.Services.AddAuthorization(options =>
{
    //Admin and Mentor can manage Learning Tasks, but the Trainee can only view them
    options.AddPolicy("CanManageTasks", policy => policy.RequireRole("Admin","Mentor"));

    //Only Trainees can submit / delete assignments
    options.AddPolicy("canManageAssignment", policy => policy.RequireRole("Trainee"));

    //Admins and Mentors can view submissions, Trainees can view their own
    //(we allow all three roles to pass the initial gate)
    options.AddPolicy("canViewSubmissions", policy => policy.RequireRole("Admin","Mentor","Trainee"));

    //Only Admins can register new Trainess and Mentors
    options.AddPolicy("canManageProfiles", policy => policy.RequireRole("Admin"));   

    //Mentors can create reviews, everyone can view them
    options.AddPolicy("CanWriteReviews", policy => policy.RequireRole("Mentor"));

    //Mentors can assign tasks, everyone else can only view them
    options.AddPolicy("CanAssignTasks", policy => policy.RequireRole("Mentor"));

}
);


//Complex Swagger JWt Support
builder.Services.AddOpenApiDocument(config =>
{
    config.AddSecurity(
        "JWT", Enumerable.Empty<String>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = NSwag.OpenApiSecurityApiKeyLocation.Header
        }
    );
}
);


//Configure CORS
builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "ReactPolicy",
            policy =>
            {
                policy.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:5173"
                ).AllowAnyHeader().AllowAnyMethod();
            });
    }
);


//Logger configs to remove unwanted providers and only add a console for logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
options.IncludeScopes = true);



var app = builder.Build();

app.UseCors("ReactPolicy");

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();



//calls the healthchecks for all when a user hits /heath/ready or live 
//Liveliness
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
   Predicate = _ => false // Just confirms the process is responding and ignore background things
});

//Readiness
app.MapHealthChecks("/health/ready", new HealthCheckOptions 
{
    Predicate = check => check.Tags.Contains("ready"), //Execute only the infrastruture with tags as ready
    ResponseWriter = async (context, report) => //.NET simply returns HEalthy or Unhealthy, so we overwrite it to get a more detailed response
    {
        context.Response.ContentType = "application/json";
        //It takes the response of all 4 checks and creates a integrated report
        var payload = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
                // Safe boundary: Exception details are excluded to prevent data leaks
            })
        };

        var jsonResponse = JsonSerializer.Serialize(payload, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });

        await context.Response.WriteAsync(jsonResponse);
    }
});

// Configure HTTP request pipeline only during production.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
    

// Generate OpenAPI document
app.UseOpenApi();

// Swagger UI
app.UseSwaggerUi();

app.MapControllers();


app.Run();