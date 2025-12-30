using MediatR;
using MongoDB.Bson;
using SmartCity.Application.Features.IoT.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.IoT.Commands;

public class RegisterMeterCommandHandler : IRequestHandler<RegisterMeterCommand, string>
{
    private readonly IAsyncRepository<UtilityMeter> _repository;

    public RegisterMeterCommandHandler(IAsyncRepository<UtilityMeter> repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(RegisterMeterCommand request, CancellationToken cancellationToken)
    {
        var meter = new UtilityMeter(
            request.SerialNumber,
            request.Type,
            ObjectId.Parse(request.CitizenId),
            request.Location,
            request.AlertThreshold
        );

        await _repository.AddAsync(meter);
        return meter.Id.ToString();
    }
}
