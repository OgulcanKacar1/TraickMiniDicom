namespace TraickMiniDicom.Services
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        Guid? OrganizationId { get; }
        string Role { get; }
    }
}