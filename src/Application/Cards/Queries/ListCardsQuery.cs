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
    public sealed record ListCardsQuery(SearchCardDTO SearchCardDTO) : IApiRequest<PaginatedApiResult<GetCardDTO>>;

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

            source = FilterByName(request.SearchCardDTO, source);
            source = FilterByColor(request.SearchCardDTO, source);
            source = FilterByStatus(request.SearchCardDTO, source);
            source = FilterByDate(request.SearchCardDTO, source);
            source = SortBy(request.SearchCardDTO, source);

            var cards = await source
                .Include(s => s.User)
                .ProjectTo<GetCardDTO>(_mapperCfg)
                .ToPaginatedResultAsync(request.SearchCardDTO.PageIndex, request.SearchCardDTO.PageSize, cancellationToken);

            return new OkObjectResult(cards);
        }

        private IQueryable<Card> FilterByName(SearchCardDTO searchCardDTO, IQueryable<Card> source)
        {
            if (!string.IsNullOrWhiteSpace(searchCardDTO.Name))
            {
                source = source.Where(s => s.Name.ToLower().Contains(searchCardDTO.Name.ToLower()));
            }

            return source;
        }

        private IQueryable<Card> FilterByColor(SearchCardDTO searchCardDTO, IQueryable<Card> source)
        {
            if (!string.IsNullOrWhiteSpace(searchCardDTO.Color))
            {
                source = source.Where(s => !string.IsNullOrWhiteSpace(s.Color) && s.Color.ToLower() == searchCardDTO.Color);
            }

            return source;
        }

        private IQueryable<Card> FilterByStatus(SearchCardDTO searchCardDTO, IQueryable<Card> source)
        {
            if (searchCardDTO.Status != null)
            {
                source = source.Where(s => s.Status == searchCardDTO.Status);
            }

            return source;
        }

        private IQueryable<Card> FilterByDate(SearchCardDTO searchCardDTO, IQueryable<Card> source)
        {
            if (searchCardDTO.FromDate != null && searchCardDTO.ToDate != null)
            {
                source = source.Where(s => s.CreatedAt >= searchCardDTO.FromDate && s.CreatedAt <= searchCardDTO.ToDate);
                return source;
            }
            else if (searchCardDTO.FromDate != null && searchCardDTO.ToDate == null)
            {
                source = source.Where(s => s.CreatedAt >= searchCardDTO.FromDate);
                return source;
            }
            else if (searchCardDTO.ToDate != null && searchCardDTO.FromDate == null)
            {
                source = source.Where(s => s.CreatedAt <= searchCardDTO.ToDate);
                return source;
            }
            else
            {
                return source;
            }
        }

        private IQueryable<Card> SortBy(SearchCardDTO searchCardDTO, IQueryable<Card> source)
        {
            if (searchCardDTO.SortBy != null)
            {
                switch (searchCardDTO.SortBy)
                {
                    case CardSortBy.Name:
                        source = source.OrderBy(s => s.Name);
                        break;
                    case CardSortBy.Color:
                        source = source.OrderBy(s => s.Color);
                        break;
                    case CardSortBy.Status:
                        source = source.OrderBy(s => s.Status);
                        break;
                    case CardSortBy.Date:
                        source = source.OrderBy(s => s.CreatedAt);
                        break;

                }
            }

            return source;
        }
    }
}
