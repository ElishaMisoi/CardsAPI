using Application.Cards.Commands.DTOs;
using Application.Cards.Queries.DTOs;
using Application.Common.CommandHandlers;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Cards.Commands
{
    public sealed record UpdateCardCommand(Guid Id, UpdateCardDTO UpdateCardDTO) : IApiRequest<GetCardDTO>;

    public sealed class UpdateCardCommandHandler : IApiRequestHandler<UpdateCardCommand, GetCardDTO>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateCardCommandHandler
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

        public async Task<ActionResult<GetCardDTO>> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
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

            if ((new Guid(currentUserId)) != existingCard!.UserId)
            {
                return new UnauthorizedObjectResult($"User is not authorized to perform this action");
            }

            existingCard.Name = request.UpdateCardDTO.Name ?? existingCard.Name;
            existingCard.Description = request.UpdateCardDTO.Description ?? existingCard.Description;
            existingCard.Color = request.UpdateCardDTO.Color ?? existingCard.Color;
            existingCard.Status = request.UpdateCardDTO.StatusId ?? existingCard.Status;

            _dbContext.Cards.Update(existingCard);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<Card, GetCardDTO>(existingCard);

            return new OkObjectResult(response);
        }
    }
}
