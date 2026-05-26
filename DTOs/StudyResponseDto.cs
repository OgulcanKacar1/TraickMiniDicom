namespace TraickMiniDicom.DTOs;

public class StudyResponseDto
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string StudyInstanceUID { get; set; } = string.Empty;
    public string Modality { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public string Resolution { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    
    // İlişkili dosyaların da sadece DTO'ları dönecek
    public List<StudyFileDto> DicomFiles { get; set; } = new();
}
