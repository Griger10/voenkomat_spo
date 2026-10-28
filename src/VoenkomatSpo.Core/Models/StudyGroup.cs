namespace VoenkomatSpo.Core.Models;

public class StudyGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Department { get; set; } = null!;
    public int GraduationYear { get; set; }
    public ICollection<Student> Students { get; set; } = [];
}
