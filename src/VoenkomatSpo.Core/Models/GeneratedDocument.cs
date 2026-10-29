namespace VoenkomatSpo.Core.Models;

public class GeneratedDocument
{
    public int Id { get; set; }
    public string Number { get; set; } = "";
    public int FormNumber { get; set; }
    public int Year { get; set; }
    public string Subject { get; set; } = "";
    public string Details { get; set; } = "";
    public string CommissariatName { get; set; } = "";
    public string CreatorLogin { get; set; } = "";
    public DateTime FormedAt { get; set; }
    public bool IsReconciled { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string PdfPath { get; set; } = "";
    public int? StudentId { get; set; }
    public int? CommissariatId { get; set; }
    public int? ReportId { get; set; }
    public int FormedBy { get; set; }
    public Student? Student { get; set; }
    public MilitaryCommissariat? Commissariat { get; set; }
    public Report? Report { get; set; }
    public User Former { get; set; } = null!;
}
