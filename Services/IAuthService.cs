using TraickMiniDicom.DTOs;
using TraickMiniDicom.Responses;

namespace TraickMiniDicom.Services;

public interface IAuthService
{
    Task<ApiResponse<string>> RegisterAsync(UserRegisterDto request);
    Task<ApiResponse<string>> LoginAsync(UserLoginDto request);
}