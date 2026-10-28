using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Configurations;

public sealed class StudyGroupConfiguration : IEntityTypeConfiguration<StudyGroup>
{
    public void Configure(EntityTypeBuilder<StudyGroup> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(30).IsRequired();
        b.Property(x => x.Department).HasMaxLength(200).IsRequired();
        b.HasIndex(x => new { x.Name, x.GraduationYear }).IsUnique();
    }
}
