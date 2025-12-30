using MediatR;
using SmartCity.Application.Features.Services.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Services.Queries;

public record GetUserRequestsQuery(string UserId) : IRequest<List<ServiceRequestDto>>;

public class GetUserRequestsQueryHandler : IRequestHandler<GetUserRequestsQuery, List<ServiceRequestDto>>
{
    private readonly IAsyncRepository<ServiceRequest> _requestRepository;
    private readonly IAsyncRepository<ServiceDefinition> _serviceRepository;

    public GetUserRequestsQueryHandler(IAsyncRepository<ServiceRequest> requestRepository, IAsyncRepository<ServiceDefinition> serviceRepository)
    {
        _requestRepository = requestRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<List<ServiceRequestDto>> Handle(GetUserRequestsQuery request, CancellationToken cancellationToken)
    {
        // This is inefficient (N+1 prob), ideally use aggregation or join or cache services
        // For MongoDB, we often duplicate ServiceName in Request or do a lookup.
        // Or fetch all services first.
        
        var requests = await _requestRepository.ListAsync(r => r.CitizenId == MongoDB.Bson.ObjectId.Parse(request.UserId));
        
        // Fetch service names - optimization: fetch unique service IDs
        var serviceIds = requests.Select(r => r.ServiceId).Distinct().ToList();
        // Since repo generic ListAsync takes predicate, hard to do "In" query easily without specific implementation
        // We will fetch all or iterate. For prototype/MVP, simple iteration or caching is okay.
        // Let's assume we can fetch all services or just map what we have.
        
        // Better: Fetch services in loop or use a cache.
        // Let's just return ServiceId for now and maybe fetch individual if needed, or fetch all services since list is small.
        var services = await _serviceRepository.ListAllAsync();
        var serviceMap = services.ToDictionary(s => s.Id, s => s.Name);

        return requests.Select(r => new ServiceRequestDto(
            r.Id.ToString(),
            r.ServiceId.ToString(),
            serviceMap.ContainsKey(r.ServiceId) ? serviceMap[r.ServiceId] : "Unknown",
            r.Status.ToString(),
            r.CreatedAt,
            r.SubmissionData
        )).ToList();
    }
}
