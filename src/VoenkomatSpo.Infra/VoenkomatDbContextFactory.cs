using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VoenkomatSpo.Infra;

public sealed class VoenkomatDbContextFactory : IDesignTimeDbContextFactory<VoenkomatDbContext>, IDbContextFactory<VoenkomatDbContext>
{
    public VoenkomatDbContext CreateDbContext(string[] args) => CreateDbContext();

    public VoenkomatDbContext CreateDbContext() => new(new DbContextOptionsBuilder<VoenkomatDbContext>()
        .UseNpgsql(DatabaseConnectionSettings.LoadConnectionString()).Options);
}
