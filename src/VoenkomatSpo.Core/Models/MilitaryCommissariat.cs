namespace VoenkomatSpo.Core.Models;

public class MilitaryCommissariat
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public ICollection<PersonalCard> PersonalCards { get; set; } = [];
    public ICollection<Report> Reports { get; set; } = [];
    public ICollection<ChangeEvent> ChangeEvents { get; set; } = [];
}
