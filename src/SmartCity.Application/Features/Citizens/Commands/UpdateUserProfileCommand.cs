using MediatR;
using SmartCity.Application.Features.Citizens.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Citizens.Commands;

// Command definition reused from DTOs file or defined here if preferred. 
// For clarity, I'll define a separate command class usually, but DTO approach is fine if small.
// Let's use the one defined in DTOs but I need to include the UserId which comes from Claims usually.

public record UpdateUserProfileCommand(string UserId, string FullName, string PhoneNumber) : IRequest<bool>;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IAsyncRepository<User> _userRepository;

    public UpdateUserProfileCommandHandler(IAsyncRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return false;

        user.UpdateProfile(request.FullName, request.PhoneNumber);
        
        await _userRepository.UpdateAsync(user);
        
        return true; 
    }
}
