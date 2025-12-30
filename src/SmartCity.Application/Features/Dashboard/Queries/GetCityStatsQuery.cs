using MediatR;
using SmartCity.Application.Features.Dashboard.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Dashboard.Queries;

public record GetCityStatsQuery : IRequest<CityStatsDto>;

public class GetCityStatsQueryHandler : IRequestHandler<GetCityStatsQuery, CityStatsDto>
{
    private readonly IAsyncRepository<User> _userRepository;
    private readonly IAsyncRepository<Complaint> _complaintRepository;
    private readonly IAsyncRepository<ServiceRequest> _serviceRequestRepository;
    private readonly IAsyncRepository<UtilityMeter> _meterRepository;

    public GetCityStatsQueryHandler(
        IAsyncRepository<User> userRepository,
        IAsyncRepository<Complaint> complaintRepository,
        IAsyncRepository<ServiceRequest> serviceRequestRepository,
        IAsyncRepository<UtilityMeter> meterRepository)
    {
        _userRepository = userRepository;
        _complaintRepository = complaintRepository;
        _serviceRequestRepository = serviceRequestRepository;
        _meterRepository = meterRepository;
    }

    public async Task<CityStatsDto> Handle(GetCityStatsQuery request, CancellationToken cancellationToken)
    {
        var totalCitizens = (await _userRepository.ListAsync(u => u.Roles.Contains("Citizen"))).Count;
        
        var activeComplaints = (await _complaintRepository.ListAsync(c => 
            c.Status == ComplaintStatus.Submitted || c.Status == ComplaintStatus.InProgress)).Count;
            
        var pendingRequests = (await _serviceRequestRepository.ListAsync(r => 
            r.Status == ServiceRequestStatus.Pending)).Count;
            
        var totalMeters = (await _meterRepository.ListAllAsync()).Count;

        // Get Recent Activities (Top 5)
        var recentComplaints = (await _complaintRepository.ListAllAsync())
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => new RecentActivityDto($"New Complaint: {c.Category}", c.CreatedAt, "Complaint"));

        var recentRequests = (await _serviceRequestRepository.ListAllAsync())
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(r => new RecentActivityDto($"Service Request Submitted", r.CreatedAt, "ServiceRequest"));

        var recentActivities = recentComplaints.Concat(recentRequests)
            .OrderByDescending(a => a.Timestamp)
            .Take(5)
            .ToList();

        return new CityStatsDto(totalCitizens, activeComplaints, pendingRequests, totalMeters, recentActivities);
    }
}
