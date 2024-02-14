using Domain.Common.Enums;
using Domain.Data.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tests
{
    public static class MockJwtToken
    {
        public static string MockToken(UserRole role = UserRole.Admin)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "testuser@cardsapi.com",
                FirstName = "Test",
                LastName = "User",
                Role = role
            };

            List<Claim> claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("name", $"{user.FirstName} {user.LastName}"),
                new Claim("email", user.Email),
                new Claim("role", user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ioljyUPh1Wg54fSo14J9timRmCGJHtpdD4BKzdWqmfDwrFTAfIu1OYT2BwdoZAqy"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
