using Application.Common.CommandHandlers;
using Application.Common.Interfaces;
using Application.Users.Queries.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries
{
    public sealed record ListUsersQuery : IApiRequest<IEnumerable<GetUserDTO>>;

    public sealed class ListUsersQueryHandler : IApiRequestHandler<ListUsersQuery, IEnumerable<GetUserDTO>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IConfigurationProvider _mapperCfg;

        public ListUsersQueryHandler(IApplicationDbContext dbContext, IConfigurationProvider mapperCfg)
        {
            _dbContext = dbContext;
            _mapperCfg = mapperCfg;
        }

        public async Task<ActionResult<IEnumerable<GetUserDTO>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _dbContext.Users.ProjectTo<GetUserDTO>(_mapperCfg).ToListAsync();

            return new OkObjectResult(users);
        }
    }
}
