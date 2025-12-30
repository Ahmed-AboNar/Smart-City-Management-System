using MediatR;
using SmartCity.Application.Common.Interfaces;
using SmartCity.Application.Features.Auth.DTOs;

namespace SmartCity.Application.Features.Auth.Queries;

public record LoginUserQuery(string Email, string Password) : IRequest<AuthResponse>;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public LoginUserQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request.Email, request.Password);
    }
}
