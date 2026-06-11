using TraineeManagement.myapp.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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



//Dependency Injection
builder.Services.AddScoped<ITraineeService,TraineeService>();
builder.Services.AddScoped<IUserService,UserService>();



//Register Authentication using JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    { 
    options.TokenValidationParameters = new TokenValidationParameters{
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
        "JWT",Enumerable.Empty<String>(),
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