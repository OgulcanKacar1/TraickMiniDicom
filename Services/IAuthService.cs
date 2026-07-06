using TraickMiniDicom.DTOs;
using TraickMiniDicom.Responses;

namespace TraickMiniDicom.Services;

public interface IAuthService
{
    Task<ServiceResult<string>> RegisterAsync(UserRegisterDto request);
    Task<ServiceResult<string>> LoginAsync(UserLoginDto request);
}