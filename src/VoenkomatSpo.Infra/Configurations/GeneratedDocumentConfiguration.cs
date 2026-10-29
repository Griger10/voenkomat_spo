using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Configurations;

public sealed class GeneratedDocumentConfiguration : IEntityTypeConfiguration<GeneratedDocument>
{
    public void Configure(EntityTypeBuilder<GeneratedDocument> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Number).HasMaxLength(80).IsRequired();
        b.HasIndex(x => x.Number).IsUnique();
        b.Property(x => x.Subject).HasMaxLength(400).IsRequired();
        b.Property(x => x.Details).HasMaxLength(400).IsRequired();
        b.Property(x => x.CommissariatName).HasMaxLength(400).IsRequired();
        b.Property(x => x.CreatorLogin).HasMaxLength(100).IsRequired();
        b.Property(x => x.PdfPath).HasMaxLength(100).IsRequired();
        b.Property(x => x.SubmittedAt).IsConcurrencyToken();
        b.Property(x => x.IsReconciled).IsConcurrencyToken();
        b.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId);
        b.HasOne(x => x.Commissariat).WithMany().HasForeignKey(x => x.CommissariatId);
        b.HasOne(x => x.Former).WithMany().HasForeignKey(x => x.FormedBy);
        b.HasOne(x => x.Report).WithMany().HasForeignKey(x => x.ReportId);
        b.HasIndex(x => x.FormedAt);
        b.ToTable(t => t.HasCheckConstraint("CK_GeneratedDocument_Form", "form_number IN (6, 10)"));
    }
}
