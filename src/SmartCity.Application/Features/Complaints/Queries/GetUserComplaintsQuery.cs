using MediatR;
using SmartCity.Application.Features.Complaints.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Complaints.Queries;

/// <summary>
/// Query to fetch complaints submitted by a specific user.
/// </summary>
public record GetUserComplaintsQuery(string UserId) : IRequest<List<ComplaintDto>>;

/// <summary>
/// Handler for GetUserComplaintsQuery.
/// Fetches data from the Repository and maps it to DTOs.
/// </summary>
public class GetUserComplaintsQueryHandler : IRequestHandler<GetUserComplaintsQuery, List<ComplaintDto>>
{
    private readonly IAsyncRepository<Complaint> _repository;

    public GetUserComplaintsQueryHandler(IAsyncRepository<Complaint> repository)
    {
        _repository = repository;
    }

    public async Task<List<ComplaintDto>> Handle(GetUserComplaintsQuery request, CancellationToken cancellationToken)
    {
        var complaints = await _repository.ListAsync(c => c.CitizenId == MongoDB.Bson.ObjectId.Parse(request.UserId));

        return complaints.Select(c => new ComplaintDto(
            c.Id.ToString(),
            c.Category,
            c.Description,
            c.Status.ToString(),
            c.AssignedDepartmentId?.ToString(),
            c.CreatedAt
        )).ToList();
    }
}
