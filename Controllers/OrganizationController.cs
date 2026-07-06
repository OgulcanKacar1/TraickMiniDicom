using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraickMiniDicom.DTOs;
using TraickMiniDicom.Extensions;
using TraickMiniDicom.Services;

namespace TraickMiniDicom.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] //Sadece Admin rolüne sahip olanlar!
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationDto request)
    {
        var result = await _organizationService.CreateOrganizationAsync(request);
        
        // Hatırlarsan bu sihirli eklentiyi biz yazmıştık (ServiceResult'ı ApiResponse'a çevirir)
        return this.ToActionResult(result); 
    }
}