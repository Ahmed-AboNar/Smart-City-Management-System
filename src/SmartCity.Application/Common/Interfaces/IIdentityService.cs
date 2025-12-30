using SmartCity.Application.Features.Auth.DTOs;

namespace SmartCity.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<AuthResponse> LoginAsync(string email, string password);
    Task<AuthResponse> RegisterAsync(string fullName, string email, string password, string nationalId, string phoneNumber);
}
