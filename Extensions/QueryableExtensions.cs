using Microsoft.EntityFrameworkCore;
using TraickMiniDicom.DTOs;
using TraickMiniDicom.Responses;
using System.Linq.Dynamic.Core;

namespace TraickMiniDicom.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedListResponse<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int limit,
        string sort,
        string sortDir,
        Dictionary<string, string>? sortFieldMapping = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var totalCount = await query.CountAsync(cancellationToken);

        // Map sort field name (camelCase API convention) to entity property name
        var sortField = "CreatedDate";
        if (sortFieldMapping != null && sortFieldMapping.TryGetValue(sort, out var mappedField))
        {
            sortField = mappedField;
        }

        var orderDirection = sortDir.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";
        query = query.OrderBy($"{sortField} {orderDirection}");

        var pageSize = Math.Clamp(limit, 1, 100);
        var currentPage = Math.Max(1, page);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedListResponse<T>
        {
            List = items,
            Pagination = new PaginationDto
            {
                PageSize = pageSize,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                TotalResults = totalCount,
                SortBy = sort,
                SortDir = orderDirection
            }
        };
    }
}