using Domain.Common.Enums;

namespace Application.Cards.Commands.DTOs
{
    public class UpdateCardDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public CardStatus? Status { get; set; }
    }
}
