using IdentityService.Application.Auth.Abstractions;
using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Common.Abstractions;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Application.Auth.Handlers;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var stored = await _userRepository.GetPasswordResetTokenAsync(request.Token, cancellationToken);
        if (stored is null || stored.Used || stored.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired reset token.");
        }

        var user = await _userRepository.GetByIdAsync(stored.UserId, cancellationToken)
                   ?? throw new UnauthorizedAccessException("User not found.");

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        stored.Used = true;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
