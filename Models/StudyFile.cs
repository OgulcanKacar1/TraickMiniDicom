namespace TraickMiniDicom.Models;

public class StudyFile
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    
    //foreign key to Study
    public int StudyId { get; set; }
        public Study Study { get; set; }
        
}