using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using VoenkomatSpo.Core.Authentication;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Authentication;

public sealed class AuthenticationService(
    IDbContextFactory<VoenkomatDbContext> contextFactory,
    IPasswordHasher passwordHasher) : IAuthenticationService
{
    private const string DummyHash = "pbkdf2-sha256$210000$UvhEHvGA1zr0UWe2sKX1tA==$H0LumF6PFzbaSvYC3K8wu7utWyWZEFaopelvvTWCszI=";

    public async Task<AuthenticatedUser?> SignInAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(login) || login.Trim().Length > 100 || string.IsNullOrEmpty(password) || password.Length > 1024)
            return null;

        try
        {
            await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
            var normalizedLogin = login.Trim();
            var user = await db.Users.Include(x => x.Role)
                .SingleOrDefaultAsync(x => x.Login == normalizedLogin, cancellationToken);
            var valid = await Task.Run(() => passwordHasher.Verify(password, user?.PasswordHash ?? DummyHash), cancellationToken);
            if (user is null || !valid)
                return null;

            cancellationToken.ThrowIfCancellationRequested();
            var now = DateTime.UtcNow;
            user.LastLoginAt = now;
            db.ActionLogs.Add(new ActionLog
            {
                Action = "Вход в систему", TableName = "User", RecordId = user.Id,
                RecordedAt = now, UserId = user.Id
            });
            await db.SaveChangesAsync(cancellationToken);
            return new AuthenticatedUser(user.Id, user.Login, user.Role.Name,
                user.Role.AccessRights.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }
        catch (Exception ex) when (ex is DbException or TimeoutException)
        {
            throw new AuthenticationUnavailableException(ex);
        }
    }
}
