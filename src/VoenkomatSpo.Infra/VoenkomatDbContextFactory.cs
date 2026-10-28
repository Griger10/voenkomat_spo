using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace VoenkomatSpo.Infra;

public sealed class VoenkomatDbContextFactory : IDesignTimeDbContextFactory<VoenkomatDbContext>
{
    public VoenkomatDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.NoClobber().TraversePath().Load();

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Voenkomat");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var portValue = RequiredVariable("POSTGRES_PORT");
            if (!int.TryParse(portValue, out var port) || port is < 1 or > 65535)
                throw new InvalidOperationException("POSTGRES_PORT must be an integer between 1 and 65535.");

            connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = RequiredVariable("POSTGRES_HOST"),
                Port = port,
                Database = RequiredVariable("POSTGRES_DB"),
                Username = RequiredVariable("POSTGRES_USER"),
                Password = RequiredVariable("POSTGRES_PASSWORD")
            }.ConnectionString;
        }

        return new VoenkomatDbContext(new DbContextOptionsBuilder<VoenkomatDbContext>()
            .UseNpgsql(connectionString).Options);
    }

    private static string RequiredVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"Set {name} in .env or the process environment.");
    }
}
