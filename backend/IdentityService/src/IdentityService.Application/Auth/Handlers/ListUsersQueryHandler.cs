using IdentityService.Application.Auth.Abstractions;
using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Auth.Dtos;
using MediatR;

namespace IdentityService.Application.Auth.Handlers;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IReadOnlyList<UserListItemDto>>
{
    private readonly IUserRepository _userRepository;

    public ListUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserListItemDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.ListAsync(cancellationToken);
        return users
            .Select(u => new UserListItemDto(u.Id, u.Email, u.Username, u.Role.ToString(), u.CreatedAt))
            .ToList();
    }
}
