using TraineeManagement.myapp.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.myapp.Data;

var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container.
builder.Services.AddControllers();
 
// NSwag
builder.Services.AddOpenApiDocument();

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

var app = builder.Build();
 
// Configure HTTP request pipeline.
app.UseHttpsRedirection();
 
// Generate OpenAPI document
app.UseOpenApi();
 
// Swagger UI
app.UseSwaggerUi();
 
app.MapControllers();
 
app.Run();