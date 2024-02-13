using Application.Common.Interfaces;
using Application.Users.Queries.DTOs;
using AutoMapper;
using Domain.Data.Entities;

namespace Application.Cards.Queries.DTOs
{
    public class GetCardDTO : IMapper<Card>
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public required string Status { get; set; }

        public Guid UserId { get; set; }
        public virtual GetUserDTO? User { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Card, GetCardDTO>();
        }
    }
}
