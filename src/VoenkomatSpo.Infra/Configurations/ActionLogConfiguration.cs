using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Configurations;

public sealed class ActionLogConfiguration : IEntityTypeConfiguration<ActionLog>
{
    public void Configure(EntityTypeBuilder<ActionLog> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Action).HasMaxLength(100).IsRequired();
        b.Property(x => x.TableName).HasMaxLength(100).IsRequired();
        b.HasOne(x => x.User).WithMany(x => x.ActionLogs).HasForeignKey(x => x.UserId);
        b.HasIndex(x => new { x.TableName, x.RecordId });
        b.HasIndex(x => x.RecordedAt);
    }
}
