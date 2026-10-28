using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Configurations;

public sealed class PersonalCardConfiguration : IEntityTypeConfiguration<PersonalCard>
{
    public void Configure(EntityTypeBuilder<PersonalCard> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.RecordCategory).HasMaxLength(60).IsRequired();
        b.Property(x => x.FitnessCategory).HasMaxLength(10);
        b.Property(x => x.MilitarySpecialty).HasMaxLength(100);
        b.Property(x => x.DocumentType).HasMaxLength(100).IsRequired();
        b.Property(x => x.DocumentNumber).HasMaxLength(40);
        b.HasOne(x => x.Student).WithOne(x => x.PersonalCard).HasForeignKey<PersonalCard>(x => x.StudentId);
        b.HasOne(x => x.Commissariat).WithMany(x => x.PersonalCards).HasForeignKey(x => x.CommissariatId);
        b.HasOne(x => x.Creator).WithMany(x => x.CreatedCards).HasForeignKey(x => x.CreatedBy);
    }
}
