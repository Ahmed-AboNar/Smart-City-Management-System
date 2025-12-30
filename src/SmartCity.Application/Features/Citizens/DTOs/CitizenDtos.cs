namespace SmartCity.Application.Features.Citizens.DTOs;

public record UserProfileDto(string Id, string FullName, string Email, string NationalId, string PhoneNumber, string DepartmentId);
public record UpdateProfileCommand(string FullName, string PhoneNumber) : MediatR.IRequest<bool>;
