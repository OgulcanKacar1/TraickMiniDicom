namespace TraickMiniDicom.Models;

public class StudyFile : BaseEntity
{
    public string FilePath { get; set; }
    
    //foreign key to Study
    public Guid StudyId { get; set; }
    public Study Study { get; set; }
        
}