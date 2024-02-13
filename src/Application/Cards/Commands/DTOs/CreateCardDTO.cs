namespace Application.Cards.Commands.DTOs
{
    public class CreateCardDTO
    {
        public required string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
    }
}
