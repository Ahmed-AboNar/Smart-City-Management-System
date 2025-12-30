namespace SmartCity.Application.Features.Auth.DTOs;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string FullName, string Email, string Password, string NationalId, string PhoneNumber);
public record AuthResponse(string Token, string RefreshToken, DateTime Expiry);
