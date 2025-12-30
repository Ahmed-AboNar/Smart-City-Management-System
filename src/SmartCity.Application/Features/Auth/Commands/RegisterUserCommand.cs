using MediatR;
using SmartCity.Application.Common.Interfaces;
using SmartCity.Application.Features.Auth.DTOs;

namespace SmartCity.Application.Features.Auth.Commands;

public record RegisterUserCommand(string FullName, string Email, string Password, string NationalId, string PhoneNumber) : IRequest<AuthResponse>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RegisterAsync(request.FullName, request.Email, request.Password, request.NationalId, request.PhoneNumber);
    }
}
