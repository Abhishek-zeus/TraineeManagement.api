using TraineeManagement.myapp.Services;
var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container.
builder.Services.AddControllers();
 
// NSwag
builder.Services.AddOpenApiDocument();

//Dependency Injection
builder.Services.AddSingleton<ITraineeService,TraineeService>(); 

var app = builder.Build();
 
// Configure HTTP request pipeline.
app.UseHttpsRedirection();
 
// Generate OpenAPI document
app.UseOpenApi();
 
// Swagger UI
app.UseSwaggerUi();
 
app.MapControllers();
 
app.Run();