using Ecommerce.BusinessObject.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Helpers;

public static class PaginationHelper
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> source, int pageIndex,
        int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<T>(items, count, pageIndex, pageSize);
    }
}