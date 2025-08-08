using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using MediatR;
using BreachApi.Extensions;
using BreachApi.Services;
using BreachApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure options
builder.Services.Configure<ExternalApiOptions>(builder.Configuration.GetSection(ExternalApiOptions.SectionName));
builder.Services.Configure<CachingOptions>(builder.Configuration.GetSection(CachingOptions.SectionName));
builder.Services.Configure<ResilienceOptions>(builder.Configuration.GetSection(ResilienceOptions.SectionName));
builder.Services.Configure<ValidationOptions>(builder.Configuration.GetSection(ValidationOptions.SectionName));

// Add services to the container.

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add MediatR for CQRS pattern
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add PDF service
builder.Services.AddScoped<IPdfService, PdfService>();

// Add breach API service with HTTP client
builder.Services.AddHttpClient<IBreachApiService, BreachApiService>();

// Add validation service
builder.Services.AddScoped<IValidationService, ValidationService>();

// Add memory caching
builder.Services.AddMemoryCache();

// Add response caching
builder.Services.AddResponseCaching();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<BreachApi.HealthChecks.ExternalApiHealthCheck>("external_api")
    .AddCheck("memory", () =>
    {
        var allocated = GC.GetTotalMemory(false);
        var threshold = 1024 * 1024 * 500; // 500 MB threshold
        return allocated < threshold ? 
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy($"Memory usage: {allocated / 1024 / 1024} MB") :
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy($"High memory usage: {allocated / 1024 / 1024} MB");
    });

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Configure structured logging
builder.Logging.Configure(options =>
{
    options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId | 
                                     ActivityTrackingOptions.TraceId | 
                                     ActivityTrackingOptions.ParentId;
});

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Breach API",
        Version = "v1",
        Description = "A comprehensive API for breach data management and analysis",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@breachapi.com"
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Add global exception handler early in the pipeline
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Breach API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Breach API Documentation";
        c.DefaultModelsExpandDepth(-1); // Hide schemas section by default
    });
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAngularApp");

// Use response caching
app.UseResponseCaching();

app.UseAuthorization();

// Map health checks
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.MapControllers();

app.Run();
