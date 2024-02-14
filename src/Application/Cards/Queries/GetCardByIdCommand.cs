using Application.Cards.Queries.DTOs;
using Application.Common.CommandHandlers;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Common.Enums;
using Domain.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Cards.Queries
{
    public sealed record GetCardByIdCommand(Guid Id) : IApiRequest<GetCardDTO>;

    public sealed class GetCardByIdCommandHandler : IApiRequestHandler<GetCardByIdCommand, GetCardDTO>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetCardByIdCommandHandler
            (
            ICurrentUserService currentUserService,
            IApplicationDbContext dbContext,
            IMapper mapper
            )
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ActionResult<GetCardDTO>> Handle(GetCardByIdCommand request, CancellationToken cancellationToken)
        {
            var existingCard = await _dbContext
                .Cards
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == request.Id);

            if (existingCard == null)
            {
                return new NotFoundObjectResult($"The card with Id {request.Id} was not found");
            }

            var currentUserId = _currentUserService.GetCurrentUserId();
            var currentUserRole = _currentUserService.GetCurrentUserRole();

            if (currentUserRole == UserRole.Member.ToString() && (new Guid(currentUserId)) != existingCard!.UserId)
            {
                return new UnauthorizedObjectResult($"User is not authorized to perform this action");
            }

            var response = _mapper.Map<Card, GetCardDTO>(existingCard);

            return new OkObjectResult(response);
        }
    }
}
