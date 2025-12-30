using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartCity.Application.Common.Exceptions;
using SmartCity.Application.Common.Interfaces;
using SmartCity.Application.Features.Auth.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Infrastructure.Identity;

/// <summary>
/// Service responsible for User Identity management, including Authentication and Registration.
/// Generates JWT Tokens for authorized users.
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly IAsyncRepository<User> _userRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(IAsyncRepository<User> userRepository, IOptions<JwtSettings> jwtSettings, ILogger<IdentityService> logger)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var users = await _userRepository.ListAsync(u => u.Email.ToLower() == normalizedEmail);
        var user = users.FirstOrDefault();

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found with email {Email}", normalizedEmail);
            throw new UnauthorizedException("Invalid credentials");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: Password mismatch for user {Email}", normalizedEmail);
            throw new UnauthorizedException("Invalid credentials");
        }

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> RegisterAsync(string fullName, string email, string password, string nationalId, string phoneNumber)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var existingUsers = await _userRepository.ListAsync(u => u.Email.ToLower() == normalizedEmail);
        if (existingUsers.Any())
        {
            throw new Exception("User with this email already exists");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(fullName, normalizedEmail, passwordHash, nationalId, phoneNumber);
        
        // Default role
        user.AssignRole("Citizen");

        await _userRepository.AddAsync(user);

        return GenerateAuthResponse(user);
    }

    private AuthResponse GenerateAuthResponse(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("id", user.Id.ToString())
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return new AuthResponse(jwt, Guid.NewGuid().ToString(), tokenDescriptor.Expires.Value);
    }
}
