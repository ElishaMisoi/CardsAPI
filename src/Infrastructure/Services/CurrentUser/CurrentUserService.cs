using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var claimsIdentity = user.Identity as ClaimsIdentity;
            var subClaim = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier);

            if (subClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            return subClaim.Value;
        }

        public string GetCurrentUserRole()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var claimsIdentity = user.Identity as ClaimsIdentity;
            var roleClaim = claimsIdentity!.FindFirst(ClaimTypes.Role);

            if (roleClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            return roleClaim.Value;
        }
    }
}
