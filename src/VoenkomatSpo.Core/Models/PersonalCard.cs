namespace VoenkomatSpo.Core.Models;

public class PersonalCard
{
    public int Id { get; set; }
    public string RecordCategory { get; set; } = null!;
    public string? FitnessCategory { get; set; }
    public string? MilitarySpecialty { get; set; }
    public string DocumentType { get; set; } = null!;
    public string? DocumentNumber { get; set; }
    public DateOnly? DocumentIssuedAt { get; set; }
    public DateOnly? DocumentRequestedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsChecked { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsArchived { get; set; }
    public DateOnly? RemovedAt { get; set; }
    public int StudentId { get; set; }
    public int CommissariatId { get; set; }
    public int CreatedBy { get; set; }
    public Student Student { get; set; } = null!;
    public MilitaryCommissariat Commissariat { get; set; } = null!;
    public User Creator { get; set; } = null!;
}
