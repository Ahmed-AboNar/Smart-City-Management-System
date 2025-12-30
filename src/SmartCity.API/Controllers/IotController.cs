using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Application.Features.IoT.DTOs;
using System.Security.Claims;

namespace SmartCity.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IotController : ControllerBase
{
    private readonly IMediator _mediator;

    // Simulate API Key auth for devices? 
    // For now, using standard Bearer or just allowing Authenticated Users (e.g. Admin simulating, or Device having token)
    
    public IotController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPost("meters")]
    public async Task<ActionResult<string>> RegisterMeter(RegisterMeterCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    // Devices might use a different auth or this endpoint is open with API Key middleware (omitted for brevity)
    [AllowAnonymous] 
    [HttpPost("readings")]
    public async Task<ActionResult> SubmitReading(SubmitReadingCommand command)
    {
        // In production: Validate API Key header
        var result = await _mediator.Send(command);
        if (!result) return NotFound("Meter not found");
        return Ok();
    }
}
