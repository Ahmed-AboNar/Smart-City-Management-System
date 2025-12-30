using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartCity.Application.Common.Interfaces;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;
using SmartCity.Infrastructure.Identity;
using SmartCity.Infrastructure.Persistence;
using SmartCity.Infrastructure.Services;

namespace SmartCity.Infrastructure;

/// <summary>
/// Expansion method to register Infrastructure services (Persistence, Identity, SignalR).
/// </summary>
public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        // Register Repositories
        services.AddScoped(typeof(IAsyncRepository<>), typeof(MongoRepository<>));

        // Register specific repositories if needed, but generic handles most for now
        // For example, we might need a way to map T to collection name dynamically or register manually
        // For simplicity, let's register the generic one, but we need to pass collection name.
        // The current Generic Repository constructor (IMongoDatabase database, string collectionName)
        // is tricky with DI unless we use a Factory or specific derived classes.
        
        // Let's refactor MongoRepository to be abstract or use a collection name provider.
        // For THIS step, I'll rewrite the DI below to validly register repositories for known types.
        
        RegisterRepositories(services);

        services.AddScoped<IAsyncRepository<UtilityMeter>>(sp => 
            new MongoRepository<UtilityMeter>(sp.GetRequiredService<IMongoDatabase>(), "UtilityMeters"));

        services.AddScoped<IAsyncRepository<MeterReading>>(sp => 
            new MongoRepository<MeterReading>(sp.GetRequiredService<IMongoDatabase>(), "MeterReadings"));

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IIdentityService, IdentityService>();
        
        services.AddSignalR();
        services.AddTransient<INotificationService, NotificationService>();

        // Add Authentication (this might fail if JwtBearer is not added, assuming package is added)
        // Note: Authentication typically configured in API Program.cs or here if we return services.
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.Secret);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience
            };
        });
        
        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IAsyncRepository<User>>(sp => 
            new MongoRepository<User>(sp.GetRequiredService<IMongoDatabase>(), "Users"));
            
        services.AddScoped<IAsyncRepository<ServiceRequest>>(sp => 
            new MongoRepository<ServiceRequest>(sp.GetRequiredService<IMongoDatabase>(), "ServiceRequests"));
            
        services.AddScoped<IAsyncRepository<Complaint>>(sp => 
            new MongoRepository<Complaint>(sp.GetRequiredService<IMongoDatabase>(), "Complaints"));

        services.AddScoped<IAsyncRepository<ServiceDefinition>>(sp => 
            new MongoRepository<ServiceDefinition>(sp.GetRequiredService<IMongoDatabase>(), "ServiceDefinitions"));
    }
}
