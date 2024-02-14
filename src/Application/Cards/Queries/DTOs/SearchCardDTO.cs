using Application.Common.Models;
using Domain.Common.Enums;

namespace Application.Cards.Queries.DTOs
{
    public class SearchCardDTO : PaginationQuery
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public CardStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public CardSortBy? SortBy { get; set; }
    }
}
