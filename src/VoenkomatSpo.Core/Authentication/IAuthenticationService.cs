namespace VoenkomatSpo.Core.Authentication;

public interface IAuthenticationService
{
    Task<AuthenticatedUser?> SignInAsync(string login, string password, CancellationToken cancellationToken = default);
}
