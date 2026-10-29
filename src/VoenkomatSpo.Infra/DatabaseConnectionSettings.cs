using Npgsql;

namespace VoenkomatSpo.Infra;

public static class DatabaseConnectionSettings
{
    public static string LoadConnectionString()
    {
        DotNetEnv.Env.NoClobber().TraversePath().Load();
        DotNetEnv.Env.NoClobber().TraversePath().Load(Path.Combine(AppContext.BaseDirectory, ".env"));
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Voenkomat");
        if (!string.IsNullOrWhiteSpace(connectionString)) return connectionString;
        if (!int.TryParse(RequiredVariable("POSTGRES_PORT"), out var port) || port is < 1 or > 65535)
            throw new InvalidOperationException("POSTGRES_PORT must be an integer between 1 and 65535.");
        return new NpgsqlConnectionStringBuilder
        {
            Host = RequiredVariable("POSTGRES_HOST"),
            Port = port,
            Database = RequiredVariable("POSTGRES_DB"),
            Username = RequiredVariable("POSTGRES_USER"),
            Password = RequiredVariable("POSTGRES_PASSWORD"),
            Timeout = 5,
            CommandTimeout = 10
        }.ConnectionString;
    }

    private static string RequiredVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return !string.IsNullOrWhiteSpace(value) ? value
            : throw new InvalidOperationException($"Set {name} in .env or the process environment.");
    }
}
