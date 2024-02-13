using Application.Common.Interfaces;
using AutoMapper;
using Domain.Data.Entities;

namespace Application.Users.Queries.DTOs
{
    public class GetUserDTO : IMapper<User>
    {
        public Guid Id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, GetUserDTO>();
        }
    }
}
