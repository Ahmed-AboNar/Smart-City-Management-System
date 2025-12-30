using AutoMapper;
using MediatR;
using SmartCity.Application.Common.Interfaces;
using SmartCity.Application.Features.Citizens.DTOs;
using SmartCity.Domain.Common.Interfaces;
using SmartCity.Domain.Entities;

namespace SmartCity.Application.Features.Citizens.Queries;

public record GetProfileQuery(string UserId) : IRequest<UserProfileDto>;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserProfileDto>
{
    private readonly IAsyncRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public GetProfileQueryHandler(IAsyncRepository<User> userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return null;

        // Manual mapping or use AutoMapper if configured
        return new UserProfileDto(
            user.Id.ToString(),
            user.FullName,
            user.Email,
            user.NationalId,
            user.PhoneNumber,
            user.DepartmentId
        );
    }
}
