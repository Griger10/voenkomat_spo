using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text.Json;
using VoenkomatSpo.Core.Authentication;

namespace VoenkomatSpo.Infra.Authentication;

[SupportedOSPlatform("windows")]
public sealed class WindowsRememberedCredentialsStore : IRememberedCredentialsStore
{
    private readonly string path;

    public WindowsRememberedCredentialsStore(string? path = null)
    {
        this.path = path ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VoenkomatSpo", "credentials.dat");
    }

    public RememberedCredentials? Load()
    {
        try
        {
            if (!File.Exists(path)) return null;
            var bytes = ProtectedData.Unprotect(File.ReadAllBytes(path), null, DataProtectionScope.CurrentUser);
            try
            {
                var credentials = JsonSerializer.Deserialize<RememberedCredentials>(bytes);
                if (credentials is not null && credentials.ExpiresAtUtc > DateTime.UtcNow &&
                    !string.IsNullOrWhiteSpace(credentials.Login) && !string.IsNullOrEmpty(credentials.Password))
                    return credentials;
                Clear();
                return null;
            }
            finally { CryptographicOperations.ZeroMemory(bytes); }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or CryptographicException or JsonException)
        {
            return null;
        }
    }

    public void Save(RememberedCredentials credentials)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(credentials);
        try
        {
            var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
            var temporaryPath = path + ".tmp";
            File.WriteAllBytes(temporaryPath, encrypted);
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally { CryptographicOperations.ZeroMemory(bytes); }
    }

    public void Clear() => File.Delete(path);
}
