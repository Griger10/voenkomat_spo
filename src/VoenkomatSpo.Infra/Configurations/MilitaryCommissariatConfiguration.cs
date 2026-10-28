using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Configurations;

public sealed class MilitaryCommissariatConfiguration : IEntityTypeConfiguration<MilitaryCommissariat>
{
    public void Configure(EntityTypeBuilder<MilitaryCommissariat> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(250).IsRequired();
        b.Property(x => x.Address).HasMaxLength(400).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(30).IsRequired();
    }
}
