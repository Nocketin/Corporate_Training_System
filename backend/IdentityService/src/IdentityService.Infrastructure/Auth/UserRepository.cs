using IdentityService.Application.Auth.Abstractions;
using IdentityService.Domain;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Auth;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _db;

    public UserRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        _db.Users.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _db.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken) =>
        await _db.Users.OrderBy(x => x.Email).ToListAsync(cancellationToken);

    public void Add(User user) => _db.Users.Add(user);

    public void Update(User user) => _db.Users.Update(user);

    public Task AddPasswordResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken) =>
        _db.PasswordResetTokens.AddAsync(token, cancellationToken).AsTask();

    public Task<PasswordResetToken?> GetPasswordResetTokenAsync(string token, CancellationToken cancellationToken) =>
        _db.PasswordResetTokens.SingleOrDefaultAsync(x => x.Token == token, cancellationToken);
}
