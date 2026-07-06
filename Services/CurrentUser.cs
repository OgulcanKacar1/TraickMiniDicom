using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TraickMiniDicom.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                // HttpContext üzerinden sisteme giren kişinin User (ClaimsPrincipal) nesnesine ulaşıyoruz.
                var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out Guid userId))

                    throw new UnauthorizedAccessException("Kullanıcı bilgisi bulunamadı.");


                return userId;
            }
        }

        public Guid? OrganizationId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue("OrganizationId");

                if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var orgId))
                    return null;//hastanesi yoksa veya okunamazsa

                return orgId;
            }
        }

        public string Role
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
                return value ?? "User"; // Varsayılan kullanıcı rolü
            }
        }
    }
}