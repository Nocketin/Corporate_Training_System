using IdentityService.Application.Auth.Abstractions;
using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Auth.Dtos;
using IdentityService.Application.Common.Abstractions;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Domain;
using MediatR;

namespace IdentityService.Application.Auth.Handlers;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, UserListItemDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserRoleCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserListItemDto> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found.");

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Role", "Role must be User or Admin.")
            });
        }

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserListItemDto(user.Id, user.Email, user.Username, user.Role.ToString(), user.CreatedAt);
    }
}
