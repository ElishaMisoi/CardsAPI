using Domain.Common.Entities;
using Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities
{
    public class Card : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = null;
        public string? Color { get; set; } = null;
        public CardStatus Status { get; set; } = default!;

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
