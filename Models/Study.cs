namespace TraickMiniDicom.Models;

public class Study: BaseEntity
{
    //Primary K.
    public String PatientName { get; set; } = string.Empty;
    public String StudyInstanceUID { get; set; } = string.Empty;
    public String Modality { get; set; } = string.Empty;
    public String Series {get; set;} = string.Empty;
    public String Resolution { get; set; } = string.Empty;
    
    
    //Navigation property for related StudyFiles
    public ICollection<StudyFile> DicomFiles { get; set; } = new List<StudyFile>();
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    
}