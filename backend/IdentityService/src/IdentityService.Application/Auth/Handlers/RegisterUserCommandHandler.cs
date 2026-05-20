using IdentityService.Application.Auth.Abstractions;
using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Auth.Dtos;
using IdentityService.Application.Common.Abstractions;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Platform.Contracts.Events;

namespace IdentityService.Application.Auth.Handlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IOutboxWriter _outboxWriter;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher<User> passwordHasher,
        IOutboxWriter outboxWriter,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _outboxWriter = outboxWriter;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException("User with this email already exists.");
        }

        var username = request.Email.Split('@')[0];
        var user = new User
        {
            Email = request.Email,
            Username = username,
            Role = UserRole.User
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _userRepository.Add(user);
        await _outboxWriter.EnqueueAsync(
            new UserCreatedEvent(user.Id, user.Email, DateTime.UtcNow),
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await _tokenService.GenerateTokensAsync(user, cancellationToken);
    }
}

