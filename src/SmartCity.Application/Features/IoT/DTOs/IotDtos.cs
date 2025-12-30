using MediatR;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.IoT.DTOs;

public record RegisterMeterCommand(string SerialNumber, MeterType Type, string CitizenId, string Location, double AlertThreshold) : IRequest<string>;
public record SubmitReadingCommand(string SerialNumber, double Value) : IRequest<bool>;
