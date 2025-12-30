using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Application.Features.Citizens.Commands;
using SmartCity.Application.Features.Citizens.DTOs;
using SmartCity.Application.Features.Citizens.Queries;
using System.Security.Claims;

namespace SmartCity.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
/// <summary>
/// API Controller for handling Citizen-related operations.
/// Includes Profile management.
/// </summary>
public class CitizenController : ControllerBase
{
    private readonly IMediator _mediator;

    public CitizenController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var profile = await _mediator.Send(new GetProfileQuery(userId));
        if (profile == null) return NotFound();

        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile(UpdateProfileCommand command)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var result = await _mediator.Send(new UpdateUserProfileCommand(userId, command.FullName, command.PhoneNumber));
        if (!result) return BadRequest("Failed to update profile");

        return NoContent();
    }
}
