using MediatR;
using Microsoft.Extensions.Logging;
using SmartCity.Application.Common.Interfaces;
using SmartCity.Application.Features.IoT.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.IoT.Commands;

public class SubmitReadingCommandHandler : IRequestHandler<SubmitReadingCommand, bool>
{
    private readonly IAsyncRepository<UtilityMeter> _meterRepository;
    private readonly IAsyncRepository<MeterReading> _readingRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SubmitReadingCommandHandler> _logger;

    public SubmitReadingCommandHandler(
        IAsyncRepository<UtilityMeter> meterRepository, 
        IAsyncRepository<MeterReading> readingRepository, 
        INotificationService notificationService,
        ILogger<SubmitReadingCommandHandler> logger)
    {
        _meterRepository = meterRepository;
        _readingRepository = readingRepository;
        _notificationService = notificationService;
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

        // 3. Check Threshold
        if (request.Value > meter.AlertThreshold)
        {
            var message = $"ALERT: Meter {meter.SerialNumber} ({meter.Type}) exceeded threshold! Value: {request.Value}, Threshold: {meter.AlertThreshold}";
            _logger.LogError(message);
            
            // Send real-time notification to the citizen owning the meter
            await _notificationService.SendNotificationAsync(meter.CitizenId.ToString(), message);
            
            // Also broadcast to Admin/Employees group? For now just broadcast to everyone for visibility in demo
            await _notificationService.BroadcastNotificationAsync($"[SYSTEM ALERT] {message}");
        }

        return true;
    }
}
