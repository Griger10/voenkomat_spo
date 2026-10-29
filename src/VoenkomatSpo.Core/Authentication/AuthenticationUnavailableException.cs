namespace VoenkomatSpo.Core.Authentication;

public sealed class AuthenticationUnavailableException(Exception innerException)
    : Exception("Сервис входа временно недоступен.", innerException);
