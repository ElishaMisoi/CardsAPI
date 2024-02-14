using Application.Cards.Commands.DTOs;
using Application.Cards.Queries.DTOs;
using Application.Common.CommandHandlers;
using Application.Common.Interfaces;
using Application.Common.Utils;
using AutoMapper;
using Domain.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Cards.Commands
{
    public sealed record CreateCardCommand(CreateCardDTO CreateCardDTO) : IApiRequest<GetCardDTO>;

    public sealed class CreateCardCommandHandler : IApiRequestHandler<CreateCardCommand, GetCardDTO>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CreateCardCommandHandler
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

        public async Task<ActionResult<GetCardDTO>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.CreateCardDTO.Color) && !HexColorValidator.isValidHexaCode(request.CreateCardDTO.Color))
            {
                return new BadRequestObjectResult($"{request.CreateCardDTO.Color} is not a valid Hex Color Code");
            }

            var currentUser = await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == new Guid(_currentUserService.GetCurrentUserId()));

            if (currentUser != null)
            {
                var newCard = new Card
                {
                    Name = request.CreateCardDTO.Name,
                    Description = request.CreateCardDTO.Description,
                    Color = request.CreateCardDTO.Color,
                    UserId = currentUser.Id,
                    User = currentUser,
                };

                _dbContext.Cards.Add(newCard);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var response = _mapper.Map<Card, GetCardDTO>(newCard);

                return new OkObjectResult(response);
            }
            else
            {
                return new UnauthorizedObjectResult("User not authorized to perform this action");
            }
        }
    }
}
