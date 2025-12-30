using MediatR;
using SmartCity.Application.Features.Services.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Services.Queries;

public record ListServicesQuery() : IRequest<List<ServiceDefinitionDto>>;

public class ListServicesQueryHandler : IRequestHandler<ListServicesQuery, List<ServiceDefinitionDto>>
{
    private readonly IAsyncRepository<ServiceDefinition> _repository;

    public ListServicesQueryHandler(IAsyncRepository<ServiceDefinition> repository)
    {
        _repository = repository;
    }

    public async Task<List<ServiceDefinitionDto>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _repository.ListAllAsync();
        
        return services.Select(s => new ServiceDefinitionDto(
            s.Id.ToString(),
            s.Name,
            s.Description,
            s.SLAInHours,
            s.RequiredDocuments
        )).ToList();
    }
}
