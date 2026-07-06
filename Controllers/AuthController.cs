using Microsoft.AspNetCore.Mvc;
using TraickMiniDicom.DTOs;
using Microsoft.AspNetCore.Authorization;
using TraickMiniDicom.Services;
using System.Security.Claims;
using TraickMiniDicom.Extensions;

namespace TraickMiniDicom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUser _currentUser;
    
    public AuthController(IAuthService authService, ICurrentUser currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
    {
        var response = await _authService.RegisterAsync(request);
        return this.ToActionResult(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto request)
    {
        var response = await _authService.LoginAsync(request);
        return this.ToActionResult(response);
    }

    [Authorize] 
    [HttpGet("secret-room")]
    public IActionResult GetSecretData()
    {
        // İçeri girmeyi başaran kişinin Token'ının içinden bilgilerini okuma
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var userId = _currentUser.UserId;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok($" Hoş geldin {email}. Sistemdeki ID numaran: {userId}, Rolün: {role}");
    }
}