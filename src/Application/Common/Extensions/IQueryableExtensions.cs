using Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Extensions
{
    public static class IQueryableExtensions
    {
        public static IEnumerable<T> ToPaginatedArray<T>(this IQueryable<T> query, int? pageIndex, int? pageSize)
        {
            return query.Paginate(pageIndex, pageSize).ToArray();
        }

        public static async Task<IEnumerable<T>> ToPaginatedArrayAsync<T>(this IQueryable<T> query, int? pageIndex, int? pageSize, CancellationToken cancellationToken = default)
        {
            return await query.Paginate(pageIndex, pageSize).ToArrayAsync(cancellationToken);
        }

        public static async Task<PaginatedApiResult<T>> ToPaginatedResultAsync<T>(this IQueryable<T> query, int? pageIndex, int? pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);

            (pageSize, pageIndex) = SanitizePaginationValues(pageIndex, pageSize);

            var items = await query.Paginate(pageIndex, pageSize).ToArrayAsync(cancellationToken);

            return PaginatedApiResult<T>.Success(items, totalCount, pageIndex.Value, pageSize.Value);
        }

        private static (int pageSize, int pageIndex) SanitizePaginationValues(int? pageIndex, int? pageSize)
        {
            pageSize = pageSize is null or < 1 ? 10 : pageSize;
            pageIndex = pageIndex is null or < 1 ? 1 : pageIndex;

            return (pageSize.Value, pageIndex.Value);
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int? pageIndex, int? pageSize)
        {
            (pageSize, pageIndex) = SanitizePaginationValues(pageIndex, pageSize);

            return query
                .Skip((pageIndex.Value - 1) * pageSize!.Value)
                .Take(pageSize.Value);
        }
    }
}
