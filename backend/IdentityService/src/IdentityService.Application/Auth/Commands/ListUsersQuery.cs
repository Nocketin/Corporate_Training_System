using IdentityService.Application.Auth.Dtos;
using MediatR;

namespace IdentityService.Application.Auth.Commands;

public record ListUsersQuery : IRequest<IReadOnlyList<UserListItemDto>>;
