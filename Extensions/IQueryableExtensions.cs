using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public static class IQueryableExtensions
{
    public static async Task<(IEnumerable<T> Items, int TotalCount)> ToPagedAsync<T>(
        this IQueryable<T> query, int page, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        return (items, totalCount);
    }
}
