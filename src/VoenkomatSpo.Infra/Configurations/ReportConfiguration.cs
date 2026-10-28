using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Configurations;

public sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> b)
    {
        b.HasKey(x => x.Id);
        b.HasOne(x => x.Commissariat).WithMany(x => x.Reports).HasForeignKey(x => x.CommissariatId);
        b.HasOne(x => x.Former).WithMany(x => x.FormedReports).HasForeignKey(x => x.FormedBy);
        b.HasIndex(x => new { x.Year, x.CommissariatId });
    }
}
