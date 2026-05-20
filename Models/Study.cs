namespace TraickMiniDicom.Models;

public class Study
{
    //Primary K.
    public int Id { get; set; }
    
    public String PatientName { get; set; } = string.Empty;
    public String StudyInstanceUID { get; set; } = string.Empty;
    public String Modality { get; set; } = string.Empty;
    public String Series {get; set;} = string.Empty;
    public String Resolution { get; set; } = string.Empty;

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    
    
    //Navigation property for related StudyFiles
    public ICollection<StudyFile> DicomFiles { get; set; } = new List<StudyFile>();
    
}