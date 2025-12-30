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
        IAsyncRepository<Complaint> complaintRepository,
        IAsyncRepository<ServiceRequest> requestRepository,
        IAsyncRepository<UtilityMeter> meterRepository,
        IAsyncRepository<MeterReading> readingRepository,
        ILogger<SmartCityContextSeed> logger)
    {
        // Seed Users
        var adminEmail = "admin@smartcity.com";
        var existingAdmin = (await userRepository.ListAsync(u => u.Email.ToLower() == adminEmail)).FirstOrDefault();
        User adminUser = existingAdmin;
        
        if (existingAdmin == null)
        {
            adminUser = new User("System Admin", adminEmail, BCrypt.Net.BCrypt.HashPassword("Admin123!"), "1000000000", "0000000000");
            adminUser.AssignRole("Admin");
            await userRepository.AddAsync(adminUser);
            logger.LogInformation("Seeded Admin User.");
        }

        // Seed Requested User
        var userEmail = "ahmdasanhabwanar@gmail.com";
        var existingUser = (await userRepository.ListAsync(u => u.Email.ToLower() == userEmail)).FirstOrDefault();
        User testUser = existingUser;

        if (existingUser == null)
        {
            testUser = new User("Ahmed Hasan Habwanar", userEmail, BCrypt.Net.BCrypt.HashPassword("Ahmed"), "1234567890", "0123456789");
            testUser.AssignRole("Citizen");
            await userRepository.AddAsync(testUser);
            logger.LogInformation("Seeded Test User: {Email}", userEmail);
        }

        // Seed Services
        var services = await serviceRepository.ListAllAsync();
        if (!services.Any())
        {
            var deptId = MongoDB.Bson.ObjectId.GenerateNewId(); // Mock Dept ID
            
            var s1 = new ServiceDefinition("Beach Access Pass", "Request a permit for beach access", deptId, 24, new List<string> { "ID Card", "Property Contract" });
            var s2 = new ServiceDefinition("Villa Maintenance", "Request plumbing or electrical maintenance", deptId, 48, new List<string> { "Villa Number" });
            var s3 = new ServiceDefinition("Cleaning Service", "Request indoor/outdoor cleaning", deptId, 12, new List<string> { "Preferred Time" });
            var s4 = new ServiceDefinition("Golf Cart Rental", "Rent a golf cart for village transport", deptId, 2, new List<string> { "License" });

            await serviceRepository.AddAsync(s1);
            await serviceRepository.AddAsync(s2);
            await serviceRepository.AddAsync(s3);
            await serviceRepository.AddAsync(s4);
            logger.LogInformation("Seeded Default Services.");
            
            services = new List<ServiceDefinition> { s1, s2, s3, s4 };
        }

        // Seed Complaints
        var complaints = await complaintRepository.ListAllAsync();
        if (!complaints.Any() && testUser != null)
        {
            var c1 = new Complaint(testUser.Id, "Maintenance", "The garden sprinkler is broken near Villa 12.");
            var c2 = new Complaint(testUser.Id, "Service", "Cleaning was requested but no one showed up.");
            
            await complaintRepository.AddAsync(c1);
            await complaintRepository.AddAsync(c2);
            logger.LogInformation("Seeded Sample Complaints.");
        }

        // Seed Service Requests
        var requests = await requestRepository.ListAllAsync();
        if (!requests.Any() && testUser != null && services.Any())
        {
            var r1 = new ServiceRequest(services[0].Id, testUser.Id, new Dictionary<string, object> { { "PropertyNumber", "V12" } });
            r1.UpdateStatus(ServiceRequestStatus.Approved, "Approved by admin", adminUser?.Id ?? MongoDB.Bson.ObjectId.GenerateNewId());
            
            var r2 = new ServiceRequest(services[1].Id, testUser.Id, new Dictionary<string, object> { { "Issue", "Leak in kitchen" } });
            
            await requestRepository.AddAsync(r1);
            await requestRepository.AddAsync(r2);
            logger.LogInformation("Seeded Sample Service Requests.");
        }

        // Seed Utility Meters
        var meters = await meterRepository.ListAllAsync();
        if (!meters.Any() && testUser != null)
        {
            var m1 = new UtilityMeter("E-1001", MeterType.Electricity, testUser.Id, "Villa 12 - Main Board", 50.0);
            var m2 = new UtilityMeter("W-2001", MeterType.Water, testUser.Id, "Villa 12 - Garden", 100.0);
            
            await meterRepository.AddAsync(m1);
            await meterRepository.AddAsync(m2);
            
            // Seed Readings
            for (int i = 0; i < 10; i++)
            {
                await readingRepository.AddAsync(new MeterReading(m1.Id, 10 + (i * 2.5)));
                await readingRepository.AddAsync(new MeterReading(m2.Id, 5 + (i * 1.2)));
            }
            logger.LogInformation("Seeded Utility Meters and Readings.");
        }
    }
}
