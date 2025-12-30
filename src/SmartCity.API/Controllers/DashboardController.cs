using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Application.Features.Dashboard.DTOs;
using SmartCity.Application.Features.Dashboard.Queries;

namespace SmartCity.API.Controllers;

[Authorize(Roles = "Admin,Employee")]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<CityStatsDto>> GetStats()
    {
        var result = await _mediator.Send(new GetCityStatsQuery());
        return Ok(result);
    }
}
