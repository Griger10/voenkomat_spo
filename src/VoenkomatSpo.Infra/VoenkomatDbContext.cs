using Microsoft.EntityFrameworkCore;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra;

public class VoenkomatDbContext(DbContextOptions<VoenkomatDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<StudyGroup> StudyGroups => Set<StudyGroup>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<PersonalCard> PersonalCards => Set<PersonalCard>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<MilitaryCommissariat> MilitaryCommissariats => Set<MilitaryCommissariat>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ChangeEvent> ChangeEvents => Set<ChangeEvent>();
    public DbSet<ActionLog> ActionLogs => Set<ActionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoenkomatDbContext).Assembly);
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.ClrType.Name);
            foreach (var property in entity.GetProperties())
            {
                var name = string.Concat(property.Name.Select((c, i) =>
                    char.IsUpper(c) && i > 0 ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
                property.SetColumnName(name);
            }
            foreach (var foreignKey in entity.GetForeignKeys())
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
