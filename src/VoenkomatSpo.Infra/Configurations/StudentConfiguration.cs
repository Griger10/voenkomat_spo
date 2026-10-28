using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.PersonalFileNumber).HasMaxLength(40).IsRequired();
        b.HasIndex(x => x.PersonalFileNumber).IsUnique();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Patronymic).HasMaxLength(100);
        b.Property(x => x.Street).HasMaxLength(200).IsRequired();
        b.Property(x => x.HouseNumber).HasMaxLength(20).IsRequired();
        b.Property(x => x.PostIndex).HasMaxLength(6).IsRequired();
        b.Property(x => x.PassportData).HasMaxLength(500).IsRequired();
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.HasOne(x => x.City).WithMany(x => x.Students).HasForeignKey(x => x.CityId);
        b.HasOne(x => x.Group).WithMany(x => x.Students).HasForeignKey(x => x.GroupId);
    }
}
