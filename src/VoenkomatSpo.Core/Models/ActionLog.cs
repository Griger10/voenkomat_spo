namespace VoenkomatSpo.Core.Models;

public class ActionLog
{
    public int Id { get; set; }
    public string Action { get; set; } = null!;
    public string TableName { get; set; } = null!;
    public int RecordId { get; set; }
    public DateTime RecordedAt { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
