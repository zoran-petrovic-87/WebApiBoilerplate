using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Helpers.Pagination;

/// <summary>
/// Extensions for <c>IQueryable</c> to support pagination. 
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Extension method that returns paginated query result.
    /// Query result is added into <c>Data</c> property of <c>PagedResult</c>.
    /// </summary>
    /// <param name="query">The query to paginate.</param>
    /// <param name="pf">The pagination filter containing page size and page number.</param>
    /// <typeparam name="T">Must be a class.</typeparam>
    /// <returns>The paged result.</returns>
    public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query, PaginationFilter pf) where
        T : class
    {
        var result = new PagedResult<T>
        {
            CurrentPage = pf.PageNumber,
            PageSize = pf.PageSize,
            RowCount = query.Count()
        };
            
        var pageCount = (double) result.RowCount / pf.PageSize;
        result.PageCount = (int) Math.Ceiling(pageCount);

        var skip = (pf.PageNumber - 1) * pf.PageSize;
        result.Data = await query.Skip(skip).Take(pf.PageSize).ToListAsync();

        return result;
    }
}