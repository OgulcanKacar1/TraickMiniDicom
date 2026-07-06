namespace TraickMiniDicom.DTOs;

public class UserRegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }

    public Guid OrganizationId { get; set; }
}