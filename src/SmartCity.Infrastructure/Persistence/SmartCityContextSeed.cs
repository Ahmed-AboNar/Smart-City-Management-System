using Microsoft.Extensions.Logging;
using SmartCity.Application.Common.Interfaces;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;
using SmartCity.Infrastructure.Identity;

namespace SmartCity.Infrastructure.Persistence;

public class SmartCityContextSeed
{
    public static async Task SeedAsync(
        IAsyncRepository<User> userRepository,
        IAsyncRepository<ServiceDefinition> serviceRepository,
        ILogger<SmartCityContextSeed> logger)
    {
        // Seed Users
        var adminEmail = "admin@smartcity.com";
        var existingAdmin = (await userRepository.ListAsync(u => u.Email.ToLower() == adminEmail)).FirstOrDefault();
        
        if (existingAdmin == null)
        {
            var adminUser = new User("System Admin", adminEmail, BCrypt.Net.BCrypt.HashPassword("Admin123!"), "1000000000", "0000000000");
            adminUser.AssignRole("Admin");
            await userRepository.AddAsync(adminUser);
            logger.LogInformation("Seeded Admin User.");
        }

        // Seed Services
        var services = await serviceRepository.ListAllAsync();
        if (!services.Any())
        {
            var deptId = MongoDB.Bson.ObjectId.GenerateNewId(); // Mock Dept ID
            
            var s1 = new ServiceDefinition("Building Permit", "Request a permit for new construction", deptId, 48, new List<string> { "ID Card", "Land Deed" });
            var s2 = new ServiceDefinition("Water Connection", "Request new water meter connection", deptId, 72, new List<string> { "ID Card", "Contract" });

            await serviceRepository.AddAsync(s1);
            await serviceRepository.AddAsync(s2);
            logger.LogInformation("Seeded Default Services.");
        }
    }
}
