using Application.Cards.Queries.DTOs;
using Application.Common.CommandHandlers;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common.Enums;
using Domain.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Cards.Queries
{
    public sealed record ListCardsQuery(int PageIndex, int PageSize) : IApiRequest<PaginatedApiResult<GetCardDTO>>;

    public sealed class ListCardsQueryHandler : IApiRequestHandler<ListCardsQuery, PaginatedApiResult<GetCardDTO>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _dbContext;
        private readonly IConfigurationProvider _mapperCfg;

        public ListCardsQueryHandler
            (
            ICurrentUserService currentUserService,
            IApplicationDbContext dbContext,
            IConfigurationProvider mapperCfg
            )
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
            _mapperCfg = mapperCfg;
        }

        public async Task<ActionResult<PaginatedApiResult<GetCardDTO>>> Handle(ListCardsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetCurrentUserId();
            var currentUserRole = _currentUserService.GetCurrentUserRole();

            IQueryable<Card> source = _dbContext.Cards.AsQueryable();

            if (currentUserRole == UserRole.Member.ToString())
            {
                source = source.Where(s => s.UserId == new Guid(currentUserId));
            }

            var cards = await source
                .Include(s => s.User)
                .ProjectTo<GetCardDTO>(_mapperCfg)
                .ToPaginatedResultAsync(request.PageIndex, request.PageSize, cancellationToken);

            return new OkObjectResult(cards);
        }
    }
}
