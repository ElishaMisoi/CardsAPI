using Application.Common.CommandHandlers;
using Application.Common.Interfaces;
using Application.Users.Commands.DTOs;
using Application.Users.Queries.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands
{
    public sealed record LoginUserCommand(LoginUserDTO LoginUserDTO) : IApiRequest<GetUserDTO>;

    public sealed class LoginUserCommandHandler : IApiRequestHandler<LoginUserCommand, GetUserDTO>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ITokenGenerator _tokenGenerator;

        public LoginUserCommandHandler
            (
            IApplicationDbContext dbContext,
            IMapper mapper,
            ITokenGenerator tokenGenerator
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<ActionResult<GetUserDTO>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(s => s.Email.ToLower() == request.LoginUserDTO.Email.ToLower());

            if (existingUser == null)
            {
                return new BadRequestObjectResult("The provided Email or Password is incorrect");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.LoginUserDTO.Password, existingUser.PasswordHash);

            if (!isPasswordValid)
            {
                return new BadRequestObjectResult("The provided Email or Password is incorrect");
            }

            string jwtToken = _tokenGenerator.CreateToken(existingUser);

            return new OkObjectResult(jwtToken);
        }
    }
}
