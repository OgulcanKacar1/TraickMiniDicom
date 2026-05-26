namespace TraickMiniDicom.DTOs;

public class StudyFileDto
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
