namespace VoenkomatSpo.Core.Models;

public class ChangeEvent
{
    public int Id { get; set; }
    public string EventType { get; set; } = null!;
    public DateOnly EventDate { get; set; }
    public DateTime? NotifiedAt { get; set; }
    public DateOnly Deadline { get; set; }
    public DateTime? SentAt { get; set; }
    public int StudentId { get; set; }
    public int? NotifiedBy { get; set; }
    public int CommissariatId { get; set; }
    public Student Student { get; set; } = null!;
    public User? Notifier { get; set; }
    public MilitaryCommissariat Commissariat { get; set; } = null!;
}
