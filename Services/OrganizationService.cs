using TraickMiniDicom.Data;
using TraickMiniDicom.DTOs;
using TraickMiniDicom.Models;
using TraickMiniDicom.Responses;

namespace TraickMiniDicom.Services;

public class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _context;

    public OrganizationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<Guid>> CreateOrganizationAsync(CreateOrganizationDto request)
    {
        // 1. DTO'dan gelen veriyi gerçek Tablo (Model) nesnesine çevir
        var newOrganization = new Organization
        {
            Name = request.Name
        };

        _context.Organizations.Add(newOrganization);
        await _context.SaveChangesAsync();

        return ServiceResult<Guid>.IsSuccess(newOrganization.Id);
    }
}