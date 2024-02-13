using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services.Authentication;
using Infrastructure.Services.CurrentUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSingleton<ITokenGenerator, TokenGenerator>();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
