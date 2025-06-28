using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using MediatR;
using BreachApi.Extensions;
using BreachApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add MediatR for CQRS pattern
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add PDF service
builder.Services.AddScoped<IPdfService, PdfService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
