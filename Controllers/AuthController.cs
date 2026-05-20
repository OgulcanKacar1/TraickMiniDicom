using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraickMiniDicom.Data;
using TraickMiniDicom.DTOs;
using TraickMiniDicom.Models;

namespace TraickMiniDicom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly AppDbContext _context;
    
    public AuthController(AppDbContext context)
    {
        _context = context;
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
}