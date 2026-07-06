using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TraickMiniDicom.Data;
using TraickMiniDicom.DTOs;
using TraickMiniDicom.Models;
using TraickMiniDicom.Responses;

namespace TraickMiniDicom.Services;


public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task<ServiceResult<string>> RegisterAsync(UserRegisterDto request)
    {
        if(await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return ServiceResult<string>.Failure("Bu e-posta adresi zaten kayıtlı.");
        } 
        
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
        var newUser = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        
        return ServiceResult<string>.IsSuccess("Kayıt başarıyla tamamlandı.");
    }
    
    public async Task<ServiceResult<string>> LoginAsync(UserLoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return ServiceResult<string>.Failure("Kullanıcı bulunamadı.");
        }
        
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ServiceResult<string>.Failure("Hatalı Şifre Girdiniz.");
        }
        
        string token = CreateToken(user);
        return ServiceResult<string>.IsSuccess(token);
        
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