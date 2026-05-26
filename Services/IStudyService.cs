using TraickMiniDicom.Models;
using TraickMiniDicom.Responses;

namespace TraickMiniDicom.Services;

public interface IStudyService
{
    Task<ApiResponse<Study>> UploadDicomAsync(IFormFile file, Guid userId);
    Task<ApiResponse<PagedListResponse<Study>>> GetAllStudiesAsync(int page, int limit, string sort, string sortDir, Guid userId);
}