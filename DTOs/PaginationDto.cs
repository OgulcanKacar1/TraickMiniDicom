namespace TraickMiniDicom.DTOs;

public class PaginationDto
{
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
    public string SortBy { get; set; } = string.Empty;
    public string SortDir {get; set;} = string.Empty;
    
}