using MediatR;
using MongoDB.Bson;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Services.Commands;

public record RequestServiceCommand(string ServiceId, Dictionary<string, object> FormData) : IRequest<string>
{
    public string CitizenId { get; set; } // Set by Controller
}

public class RequestServiceCommandHandler : IRequestHandler<RequestServiceCommand, string>
{
    private readonly IAsyncRepository<ServiceRequest> _requestRepository;
    private readonly IAsyncRepository<ServiceDefinition> _serviceRepository;

    public RequestServiceCommandHandler(IAsyncRepository<ServiceRequest> requestRepository, IAsyncRepository<ServiceDefinition> serviceRepository)
    {
        _requestRepository = requestRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<string> Handle(RequestServiceCommand request, CancellationToken cancellationToken)
    {
        var serviceId = ObjectId.Parse(request.ServiceId);
        var citizenId = ObjectId.Parse(request.CitizenId);

        var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
        if (service == null) throw new Exception("Service not found");

        var serviceRequest = new ServiceRequest(serviceId, citizenId, request.FormData);
        
        await _requestRepository.AddAsync(serviceRequest);

        return serviceRequest.Id.ToString();
    }
}
