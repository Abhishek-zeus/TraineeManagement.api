using TraineeManagement.myapp.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TraineeManagement.myapp.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using System.Text;
using TraineeManagement.myapp.Utility;
using DotNetEnv;

Env.Load(); // Loads the .env file into environment variables for Jwt
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Simple NSwag
// builder.Services.AddOpenApiDocument();

//Dependency Injection
// builder.Services.AddSingleton<ITraineeService,TraineeService>(); 

//EFCORE implementation
// builder.Services.AddDbContext<AppDbContext>(
//     options => options.UseInMemoryDatabase("TraineeManagementDb")
// );


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

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
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
builder.Logging.AddConsole();


var app = builder.Build();

app.UseCors("ReactPolicy");

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();


// Configure HTTP request pipeline.
app.UseHttpsRedirection();

// Generate OpenAPI document
app.UseOpenApi();

// Swagger UI
app.UseSwaggerUi();

app.MapControllers();


app.Run();