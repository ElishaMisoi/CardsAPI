using Application.Common.CommandHandlers;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Queries.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Application.Users.Queries
{
    public sealed record ListUsersQuery(int PageIndex, int PageSize) : IApiRequest<PaginatedApiResult<GetUserDTO>>;

    public sealed class ListUsersQueryHandler : IApiRequestHandler<ListUsersQuery, PaginatedApiResult<GetUserDTO>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IConfigurationProvider _mapperCfg;

        public ListUsersQueryHandler(IApplicationDbContext dbContext, IConfigurationProvider mapperCfg)
        {
            _dbContext = dbContext;
            _mapperCfg = mapperCfg;
        }

        public async Task<ActionResult<PaginatedApiResult<GetUserDTO>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _dbContext
                .Users
                .ProjectTo<GetUserDTO>(_mapperCfg)
                .ToPaginatedResultAsync(request.PageIndex, request.PageSize, cancellationToken);

            return new OkObjectResult(users);
        }
    }
}
