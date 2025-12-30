using MediatR;
using MongoDB.Bson;
using SmartCity.Application.Features.Services.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Services.Commands;

public class UpdateRequestStatusCommandHandler : IRequestHandler<UpdateRequestStatusCommand, bool>
{
    private readonly IAsyncRepository<ServiceRequest> _repository;

    public UpdateRequestStatusCommandHandler(IAsyncRepository<ServiceRequest> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.RequestId);
        if (entity == null) return false;

        entity.UpdateStatus(request.NewStatus, request.Remarks, ObjectId.Parse(request.ChangedByUserId));

        await _repository.UpdateAsync(entity);

        return true;
    }
}
