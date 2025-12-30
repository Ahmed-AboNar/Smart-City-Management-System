using MediatR;
using MongoDB.Bson;
using SmartCity.Application.Features.Services.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Services.Commands;

public class CreateServiceDefinitionCommandHandler : IRequestHandler<CreateServiceDefinitionCommand, string>
{
    private readonly IAsyncRepository<ServiceDefinition> _repository;

    public CreateServiceDefinitionCommandHandler(IAsyncRepository<ServiceDefinition> repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(CreateServiceDefinitionCommand request, CancellationToken cancellationToken)
    {
        // Parse DepartmentId - assuming it's an ObjectId or String depending on Department Entity
        // In ERD DepartmentId is ID. Let's assume it's ObjectId for now
        
        ObjectId deptId = ObjectId.Parse(request.DepartmentId);

        var entity = new ServiceDefinition(
            request.Name,
            request.Description,
            deptId,
            request.SLAInHours,
            request.RequiredDocuments
        );

        await _repository.AddAsync(entity);

        return entity.Id.ToString();
    }
}
