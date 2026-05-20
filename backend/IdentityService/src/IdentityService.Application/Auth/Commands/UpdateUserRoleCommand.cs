using IdentityService.Application.Auth.Dtos;
using MediatR;

namespace IdentityService.Application.Auth.Commands;

public record UpdateUserRoleCommand(Guid UserId, string Role) : IRequest<UserListItemDto>;
