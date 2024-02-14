namespace Application.Common.Models
{
    public record PaginatedApiResult<T>
    {
        public T[] Data { get; set; } = Array.Empty<T>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public static PaginatedApiResult<T> Success(T[] data, int totalCount, int pageIndex, int pageSize)
        {
            return new PaginatedApiResult<T>
            {
                Data = data,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = (pageIndex * pageSize) < totalCount,
            };
        }
    }
}
