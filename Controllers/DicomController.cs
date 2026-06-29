using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TraickMiniDicom.Services;
using System.Security.Claims;
using TraickMiniDicom.Responses;
using TraickMiniDicom.Models;

namespace TraickMiniDicom.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DicomController: ControllerBase
{
    private readonly IStudyService _studyService;
    
    public DicomController(IStudyService studyService)
    {
        _studyService = studyService;
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadDicom(IFormFile file)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized();
            
        var response = await _studyService.UploadDicomAsync(file, userId);
        
        return this.ToActionResult(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudies(
        [FromQuery] int page = 1, 
        [FromQuery] int limit = 10,
        [FromQuery] string sort = "CreatedAt",
        [FromQuery] string sortDir = "desc")
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized();

        var response = await _studyService.GetAllStudiesAsync(page, limit, sort, sortDir, userId);
        
        return this.ToActionResult(response);
    }
}