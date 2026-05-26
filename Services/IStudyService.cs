using TraickMiniDicom.Models;
using TraickMiniDicom.Responses;
using TraickMiniDicom.DTOs;

namespace TraickMiniDicom.Services;

public interface IStudyService
{
    Task<ApiResponse<StudyResponseDto>> UploadDicomAsync(IFormFile file, Guid userId);
    Task<ApiResponse<PagedListResponse<StudyResponseDto>>> GetAllStudiesAsync(int page, int limit, string sort, string sortDir, Guid userId);
}