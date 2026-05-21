namespace TraickMiniDicom.Models;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    
    public string Role { get; set; } = "User";
    
    public ICollection<Study> Studies { get; set; } = new List<Study>();
}