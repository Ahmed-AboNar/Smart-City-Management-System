namespace SmartCity.Application.Features.Complaints.DTOs;

public record ComplaintDto(string Id, string Category, string Description, string Status, string AssignedDepartmentId, DateTime CreatedAt);
public record SubmitComplaintCommand(string Category, string Description, double Latitude, double Longitude) : MediatR.IRequest<string>
{
    public string CitizenId { get; set; }
}
