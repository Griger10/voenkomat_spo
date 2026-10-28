namespace VoenkomatSpo.Core.Models;

public class Student
{
    public int Id { get; set; }
    public string PersonalFileNumber { get; set; } = null!;
    public DateOnly? FileTransferredAt { get; set; }
    public string LastName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly BirthDate { get; set; }
    public int CityId { get; set; }
    public string Street { get; set; } = null!;
    public string HouseNumber { get; set; } = null!;
    public string PostIndex { get; set; } = null!;
    public string PassportData { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateOnly EnrolledAt { get; set; }
    public int GroupId { get; set; }
    public City City { get; set; } = null!;
    public StudyGroup Group { get; set; } = null!;
    public PersonalCard? PersonalCard { get; set; }
    public ICollection<ChangeEvent> ChangeEvents { get; set; } = [];
}
