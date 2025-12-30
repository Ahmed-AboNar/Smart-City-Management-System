using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Application.Features.Services.Commands;
using SmartCity.Application.Features.Services.DTOs;
using SmartCity.Application.Features.Services.Queries;
using System.Security.Claims;

namespace SmartCity.API.Controllers;

[Authorize]
[ApiController]
[Route("api/requests")]
public class ServiceRequestController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServiceRequestController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("my-history")]
    public async Task<ActionResult<List<ServiceRequestDto>>> GetMyHistory()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var requests = await _mediator.Send(new GetUserRequestsQuery(userId));
        return Ok(requests);
    }

    // Move Submit here if desired, otherwise keeping strictly to lifecycle management
    
    [Authorize(Roles = "Admin,Manager,Employee")]
    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateStatus(string id, UpdateRequestStatusCommand command)
    {
        if (id != command.RequestId) return BadRequest("ID mismatch");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        command.ChangedByUserId = userId;

        var result = await _mediator.Send(command);
        if (!result) return NotFound();

        return NoContent();
    }
}
