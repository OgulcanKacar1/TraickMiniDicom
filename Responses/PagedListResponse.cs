using TraickMiniDicom.DTOs;

namespace TraickMiniDicom.Responses;

public class PagedListResponse<T>
{
    public List<T> List { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}