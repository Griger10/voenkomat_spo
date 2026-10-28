namespace VoenkomatSpo.Core.Models;

public class Report
{
    public int Id { get; set; }
    public int Year { get; set; }
    public DateTime FormedAt { get; set; }
    public bool IsReconciled { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int CommissariatId { get; set; }
    public int FormedBy { get; set; }
    public MilitaryCommissariat Commissariat { get; set; } = null!;
    public User Former { get; set; } = null!;
}
