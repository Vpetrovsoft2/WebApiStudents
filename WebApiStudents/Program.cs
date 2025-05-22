using Microsoft.EntityFrameworkCore;
using WebApiStudents.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student API",
        Version = "v1",
        Description = "API для управления студентами и факультетами"
    });

    c.EnableAnnotations();
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseSwagger(options => options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
});

app.UseAuthorization();
app.MapControllers();

// Apply migrations at startup with retry logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<AppDbContext>();
    
    var retryCount = 0;
    var maxRetries = 5;
    
    while (retryCount < maxRetries)
    {
        try
        {
            logger.LogInformation("Attempting to apply migrations...");
            db.Database.Migrate();
            logger.LogInformation("Migrations applied successfully");
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            logger.LogError(ex, "An error occurred while applying migrations. Retry {RetryCount} of {MaxRetries}", retryCount, maxRetries);
            
            if (retryCount == maxRetries)
            {
                logger.LogError("Failed to apply migrations after {MaxRetries} attempts", maxRetries);
                throw;
            }
            
            Thread.Sleep(5000); // Wait 5 seconds before retrying
        }
    }
}

app.Run();
