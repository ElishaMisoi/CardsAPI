using Application.Common.CommandHandlers;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Cards.Commands
{
    public sealed record DeleteCardCommand(Guid Id) : IApiRequest;

    public sealed class DeleteCardCommandHandler : IApiRequestHandler<DeleteCardCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _dbContext;

        public DeleteCardCommandHandler
            (
            ICurrentUserService currentUserService,
            IApplicationDbContext dbContext
            )
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
        }

        public async Task<ActionResult> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
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

            _dbContext.Cards.Remove(existingCard);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new OkResult();
        }
    }
}
