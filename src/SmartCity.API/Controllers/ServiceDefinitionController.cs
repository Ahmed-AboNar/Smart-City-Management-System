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
[Route("api/[controller]")]
public class ServiceDefinitionController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServiceDefinitionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ServiceDefinitionDto>>> ListServices()
    {
        var services = await _mediator.Send(new ListServicesQuery());
        return Ok(services);
    }

    [Authorize(Roles = "Admin,Manager")] // Simple role check
    [HttpPost]
    public async Task<ActionResult<string>> CreateService(CreateServiceDefinitionCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ListServices), new { id }, id);
    }

    [HttpPost("request")]
    public async Task<ActionResult<string>> SubmitRequest(RequestServiceCommand command)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        command.CitizenId = userId;
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}
