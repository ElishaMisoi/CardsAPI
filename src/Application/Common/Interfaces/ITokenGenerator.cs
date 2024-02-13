using Domain.Data.Entities;

namespace Application.Common.Interfaces
{
    public interface ITokenGenerator
    {
        string CreateToken(User user);
    }
}