namespace VoenkomatSpo.Core.Authentication;

public sealed record AuthenticatedUser(int Id, string Login, string Role, IReadOnlyList<string> AccessRights);
