using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using TaskManagerAPI.Middleware;
using TaskManagerAPI.Repositories;
using TaskManagerAPI.Services;

// Configure Serilog for structured logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/taskmanager-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Task Manager API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllers();

    // Add FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // Register our services (Dependency Injection)
    // Singleton because we're using in-memory storage
    builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
    builder.Services.AddScoped<ITaskService, TaskService>();

    // Add Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Task Manager API",
            Description = "A simple RESTful API for managing tasks using .NET 8",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "PathLock Assignment",
                Url = new Uri("https://github.com/yourusername/taskmanager")
            }
        });

        // Include XML comments for better API documentation
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend",
            policy =>
            {
                policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline

    // Global exception handling middleware (FIRST in pipeline)
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Serilog request logging
    app.UseSerilogRequestLogging();

    // Swagger in all environments for demo purposes
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });

    app.UseHttpsRedirection();

    app.UseCors("AllowFrontend");

    app.UseAuthorization();

    app.MapControllers();

    // Health check endpoint
    app.MapGet("/health", () => Results.Ok(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        version = "1.0.0"
    }))
    .WithName("HealthCheck")
    .WithOpenApi();

    Log.Information("Task Manager API started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
