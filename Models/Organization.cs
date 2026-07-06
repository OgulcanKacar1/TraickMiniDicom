namespace TraickMiniDicom.Models;

public class Organization : BaseEntity
{
    public string Name {get; set; } = string.Empty;


    //bir organization da birden fazla kullanıcı calısabilir
    public ICollection<User> Users {get; set;} = new List<User>();

    //organziation hastalarına ait birden fazla study olabilir
    public ICollection<Study> Studies {get; set;} = new List<Study>();

    
}