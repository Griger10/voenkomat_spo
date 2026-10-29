namespace VoenkomatSpo.Core.Authentication;

public sealed record RememberedCredentials(string Login, string Password, DateTime ExpiresAtUtc);

public interface IRememberedCredentialsStore
{
    RememberedCredentials? Load();
    void Save(RememberedCredentials credentials);
    void Clear();
}
