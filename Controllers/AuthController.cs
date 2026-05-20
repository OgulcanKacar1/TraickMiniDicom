using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraickMiniDicom.Data;
using TraickMiniDicom.DTOs;
using TraickMiniDicom.Models;
using Microsoft.AspNetCore.Authorization;



namespace TraickMiniDicom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    
    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
    {
        if(await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Bu e-posta adresi zaten kayıtlı.");
        }
        
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
        var newUser = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        
        return Ok("Kayıt başarıyla tamamlandı.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return BadRequest("Kullanıcı bulunamadı.");
        }
        
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest("Hatalı Şifre Girdiniz.");
        }
        
        string token = CreateToken(user);
        return Ok(new { token });
        
    }

    [Authorize] 
    [HttpGet("secret-room")]
    public IActionResult GetSecretData()
    {
        // İçeri girmeyi başaran kişinin Token'ının içinden bilgilerini okuma
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok($" Hoş geldin {email}. Sistemdeki ID numaran: {userId}, Rolün: {role}");
    }
    
    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        
        //appsettings.json dosyasından JWT anahtarını alarak SymmetricSecurityKey oluşturma
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("Jwt:Key").Value!));
        
        // anahtarı kullanarak dijital imza oluşturma
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        
        // token ayarları paketleme
        var token = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt:Issuer").Value,
            audience: _configuration.GetSection("Jwt:Audience").Value,
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );
        
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;

    }
    
    
}