using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Application.Features.Auth.Commands;
using SmartCity.Application.Features.Auth.DTOs;
using SmartCity.Application.Features.Auth.Queries;

namespace SmartCity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _mediator.Send(new LoginUserQuery(request.Email, request.Password));
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await _mediator.Send(new RegisterUserCommand(request.FullName, request.Email, request.Password, request.NationalId, request.PhoneNumber));
        return Ok(response);
    }
}
