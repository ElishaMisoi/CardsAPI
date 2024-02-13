using Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Card> Cards { get; }
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
