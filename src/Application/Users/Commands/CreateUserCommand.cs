using Application.Common.CommandHandlers;
using Application.Common.Interfaces;
using Application.Common.Utils;
using Application.Users.Commands.DTOs;
using Application.Users.Queries.DTOs;
using AutoMapper;
using Domain.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands
{
    public sealed record CreateUserCommand(CreateUserDTO CreateUserDTO) : IApiRequest<GetUserDTO>;

    public sealed class CreateUserCommandHandler : IApiRequestHandler<CreateUserCommand, GetUserDTO>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ActionResult<GetUserDTO>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(s => s.Email.ToLower() == request.CreateUserDTO.Email.ToLower());

            if (existingUser != null)
            {
                return new ConflictObjectResult($"User with the email {request.CreateUserDTO.Email} exists");
            }

            var isValidPassword = PasswordValidator.IsValidPassword(request.CreateUserDTO.Password);

            if (!isValidPassword)
            {
                return new BadRequestObjectResult("Password must have a digit, upper case character and a minimum of 8 characters");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.CreateUserDTO.Password);

            var newUser = new User
            {
                FirstName = request.CreateUserDTO.FirstName,
                LastName = request.CreateUserDTO.LastName,
                Email = request.CreateUserDTO.Email,
                PasswordHash = passwordHash,
                Role = request.CreateUserDTO.RoleId
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<User, GetUserDTO>(newUser);

            return new OkObjectResult(response);
        }
    }
}
