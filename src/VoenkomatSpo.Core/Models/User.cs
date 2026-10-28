namespace VoenkomatSpo.Core.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime? LastLoginAt { get; set; }
    public DateTime RegisteredAt { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public ICollection<PersonalCard> CreatedCards { get; set; } = [];
    public ICollection<Report> FormedReports { get; set; } = [];
    public ICollection<ChangeEvent> NotifiedEvents { get; set; } = [];
    public ICollection<ActionLog> ActionLogs { get; set; } = [];
}
