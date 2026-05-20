using IdentityService.Domain;

namespace IdentityService.Application.Auth.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken);

    void Add(User user);

    void Update(User user);

    Task AddPasswordResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken);

    Task<PasswordResetToken?> GetPasswordResetTokenAsync(string token, CancellationToken cancellationToken);
}

