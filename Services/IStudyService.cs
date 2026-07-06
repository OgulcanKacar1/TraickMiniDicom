using TraickMiniDicom.Models;
using TraickMiniDicom.Responses;
using TraickMiniDicom.DTOs;

namespace TraickMiniDicom.Services;

public interface IStudyService
{
    Task<ServiceResult<StudyResponseDto>> UploadDicomAsync(IFormFile file, Guid userId);
    Task<ServiceResult<PagedListResponse<StudyResponseDto>>> GetAllStudiesAsync(int page, int limit, string sort, string sortDir, Guid userId);
}