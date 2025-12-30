using MediatR;
using Microsoft.Extensions.Logging;
using SmartCity.Application.Features.IoT.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.IoT.Commands;

public class SubmitReadingCommandHandler : IRequestHandler<SubmitReadingCommand, bool>
{
    private readonly IAsyncRepository<UtilityMeter> _meterRepository;
    private readonly IAsyncRepository<MeterReading> _readingRepository;
    private readonly ILogger<SubmitReadingCommandHandler> _logger;

    public SubmitReadingCommandHandler(IAsyncRepository<UtilityMeter> meterRepository, IAsyncRepository<MeterReading> readingRepository, ILogger<SubmitReadingCommandHandler> logger)
    {
        _meterRepository = meterRepository;
        _readingRepository = readingRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(SubmitReadingCommand request, CancellationToken cancellationToken)
    {
        // 1. Find Meter
        var meters = await _meterRepository.ListAsync(m => m.SerialNumber == request.SerialNumber);
        var meter = meters.FirstOrDefault();
        if (meter == null)
        {
            _logger.LogWarning("Reading received for unknown meter: {SerialNumber}", request.SerialNumber);
            return false;
        }

        // 2. Save Reading
        var reading = new MeterReading(meter.Id, request.Value);
        await _readingRepository.AddAsync(reading);

        // 3. Check Threshold (Simple Logic: If value > threshold)
        // In reality, this might be usage-based (Value - PreviousValue > Threshold)
        // For simplicity: Alert if absolute value exceeds threshold (e.g. pressure, or instant KW) 
        // OR assuming Value is consumption since last reading.
        
        if (request.Value > meter.AlertThreshold)
        {
            _logger.LogError("ALERT: Meter {SerialNumber} exceeded threshold! Value: {Value}, Threshold: {Threshold}", meter.SerialNumber, request.Value, meter.AlertThreshold);
            // In real system: Publish Event -> Notification Service -> Send Email/SignalR
        }

        return true;
    }
}
