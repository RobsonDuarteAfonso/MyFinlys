using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereNotDeleted<T>(this IQueryable<T> query) where T : Entity
    {
        return query.Where(x => !x.IsDeleted);
    }

    public static PaginatedResult<T> ToPaginated<T>(
        this IEnumerable<T> query,
        int pageNumber,
        int pageSize) where T : Entity
    {
        var totalCount = query.Count();
        
        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
