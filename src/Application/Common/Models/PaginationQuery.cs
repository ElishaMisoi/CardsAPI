namespace Application.Common.Models
{
    public class PaginationQuery
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
