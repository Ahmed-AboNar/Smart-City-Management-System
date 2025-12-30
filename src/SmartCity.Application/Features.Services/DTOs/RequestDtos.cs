using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Services.DTOs;

public record ServiceRequestDto(string Id, string ServiceId, string ServiceName, string Status, DateTime CreatedAt, Dictionary<string, object> SubmissionData);
public record UpdateRequestStatusCommand(string RequestId, ServiceRequestStatus NewStatus, string Remarks) : MediatR.IRequest<bool>
{
    public string ChangedByUserId { get; set; }
}
