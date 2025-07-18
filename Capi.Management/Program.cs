// Import necessary namespaces for the application.
using Capi.Management.Data;
using Capi.Management.Dtos;
using Capi.Management.Middleware;
using Capi.Management.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

// Create a new web application builder.
var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured logging.
builder.Host.UseSerilog((context, configuration) => 
    // Read the Serilog configuration from the application's configuration (appsettings.json).
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the dependency injection container.
// Check if the environment is Development.
if (builder.Environment.IsDevelopment())
{
    // Use an in-memory database for development to simplify setup.
    builder.Services.AddDbContext<ApiDbContext>(options =>
        options.UseInMemoryDatabase("CapiManagement"));
}
else
{
    // Use SQL Server for production environments.
    builder.Services.AddDbContext<ApiDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Configure Redis distributed caching.
builder.Services.AddStackExchangeRedisCache(options =>
{
    // Get the Redis connection string from the configuration.
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    // Set an instance name for the cache to avoid collisions.
    options.InstanceName = "CapiManagement_";
});

// Register the ApiService and its interface for dependency injection.
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IApiProductService, ApiProductService>();
// Register the KongAdminService and configure its HttpClient.
builder.Services.AddHttpClient<IKongAdminService, KongAdminService>(client =>
{
    var kongAdminUrl = builder.Configuration["Kong:AdminUrl"];
    if (string.IsNullOrEmpty(kongAdminUrl))
    {
        throw new InvalidOperationException("Kong Admin URL is not configured.");
    }
    client.BaseAddress = new Uri(kongAdminUrl);
});

// Add and configure health checks for the application.
var healthChecksBuilder = builder.Services.AddHealthChecks()
    // Add a health check for the database context.
    .AddDbContextCheck<ApiDbContext>();

// Get the Redis connection string from the configuration.
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
// Add a health check for Redis only if a connection string is provided.
if (!string.IsNullOrEmpty(redisConnectionString))
{
    healthChecksBuilder.AddRedis(redisConnectionString);
}

// Add API explorer services for Swagger/OpenAPI.
builder.Services.AddEndpointsApiExplorer();
// Add Swagger generation services.
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// Build the web application.
var app = builder.Build();

// Configure the HTTP request pipeline.
// Check if the environment is Development.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger middleware for generating API documentation.
    app.UseSwagger();
    // Enable Swagger UI for interactive API documentation.
    app.UseSwaggerUI();
}

// Add Serilog request logging to the pipeline to log all incoming requests.
app.UseSerilogRequestLogging();

// Add the API key middleware to the pipeline for request authentication.
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

// Map the health check endpoint to /healthz.
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    // Always run all health checks.
    Predicate = _ => true,
    // Use a custom response writer for a more detailed health check UI.
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Run the application.
app.Run();
