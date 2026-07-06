using TraickMiniDicom.DTOs;
using TraickMiniDicom.Responses;

namespace TraickMiniDicom.Services;

public interface IOrganizationService
{
    Task<ServiceResult<Guid>> CreateOrganizationAsync(CreateOrganizationDto request);
}