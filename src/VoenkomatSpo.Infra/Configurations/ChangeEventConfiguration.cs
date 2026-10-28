using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Configurations;

public sealed class ChangeEventConfiguration : IEntityTypeConfiguration<ChangeEvent>
{
    public void Configure(EntityTypeBuilder<ChangeEvent> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.EventType).HasMaxLength(100).IsRequired();
        b.HasOne(x => x.Student).WithMany(x => x.ChangeEvents).HasForeignKey(x => x.StudentId);
        b.HasOne(x => x.Notifier).WithMany(x => x.NotifiedEvents).HasForeignKey(x => x.NotifiedBy);
        b.HasOne(x => x.Commissariat).WithMany(x => x.ChangeEvents).HasForeignKey(x => x.CommissariatId);
        b.HasIndex(x => x.Deadline);
    }
}
