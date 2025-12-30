using MediatR;
using MongoDB.Bson;
using SmartCity.Application.Features.Complaints.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Complaints.Commands;

public class SubmitComplaintCommandHandler : IRequestHandler<SubmitComplaintCommand, string>
{
    private readonly IAsyncRepository<Complaint> _repository;

    public SubmitComplaintCommandHandler(IAsyncRepository<Complaint> repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(SubmitComplaintCommand request, CancellationToken cancellationToken)
    {
        var citizenId = ObjectId.Parse(request.CitizenId);
        
        var complaint = new Complaint(citizenId, request.Category, request.Description);
        
        // Logic for Auto-Assignment could go here or in a Domain Service
        // For now, simple logic based on Category
        if (request.Category == "Water")
        {
            // complaint.AssignToDepartment(...);
        }

        await _repository.AddAsync(complaint);
        return complaint.Id.ToString();
    }
}
