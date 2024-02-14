using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private ApplicationDbContext? _dbContext;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            try
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                             typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services
                    .AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<ApplicationDbContext>((sp, options) =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting")
                        .UseInternalServiceProvider(sp);
                    });

                    var serviceProvider = services.BuildServiceProvider();

                    using var scope = serviceProvider.CreateScope();

                    var scopedServices = scope.ServiceProvider;

                    _dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();
                });
            }
            catch { }
        }

        ~CustomWebApplicationFactory()
        {
            _dbContext?.Database.EnsureDeleted();

            _dbContext?.Dispose();
        }
    }
}
