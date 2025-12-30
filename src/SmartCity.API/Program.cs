using SmartCity.Application;
using SmartCity.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userRepo = services.GetRequiredService<SmartCity.Domain.Common.Interfaces.IAsyncRepository<SmartCity.Domain.Entities.User>>();
        var serviceRepo = services.GetRequiredService<SmartCity.Domain.Common.Interfaces.IAsyncRepository<SmartCity.Domain.Entities.ServiceDefinition>>();
        var complaintRepo = services.GetRequiredService<SmartCity.Domain.Common.Interfaces.IAsyncRepository<SmartCity.Domain.Entities.Complaint>>();
        var requestRepo = services.GetRequiredService<SmartCity.Domain.Common.Interfaces.IAsyncRepository<SmartCity.Domain.Entities.ServiceRequest>>();
        var meterRepo = services.GetRequiredService<SmartCity.Domain.Common.Interfaces.IAsyncRepository<SmartCity.Domain.Entities.UtilityMeter>>();
        var readingRepo = services.GetRequiredService<SmartCity.Domain.Common.Interfaces.IAsyncRepository<SmartCity.Domain.Entities.MeterReading>>();
        var logger = services.GetRequiredService<ILogger<SmartCity.Infrastructure.Persistence.SmartCityContextSeed>>();

        await SmartCity.Infrastructure.Persistence.SmartCityContextSeed.SeedAsync(
            userRepo, 
            serviceRepo, 
            complaintRepo, 
            requestRepo, 
            meterRepo, 
            readingRepo, 
            logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
app.UseMiddleware<SmartCity.API.Middlewares.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Redirect root to Swagger UI
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<SmartCity.Infrastructure.Services.NotificationHub>("/notifications");

app.Run();
