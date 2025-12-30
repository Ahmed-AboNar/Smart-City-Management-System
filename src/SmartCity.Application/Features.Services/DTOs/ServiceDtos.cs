using MongoDB.Bson;

namespace SmartCity.Application.Features.Services.DTOs;

public record ServiceDefinitionDto(string Id, string Name, string Description, int SLAInHours, List<string> RequiredDocuments);
public record CreateServiceDefinitionCommand(string Name, string Description, string DepartmentId, int SLAInHours, List<string> RequiredDocuments) : MediatR.IRequest<string>;
