using Api.Common;
using Application.Common.Models;
using Application.Users.Commands;
using Application.Users.Commands.DTOs;
using Application.Users.Queries;
using Application.Users.Queries.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class UsersController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<GetUserDTO>> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            return await Mediator.Send(new CreateUserCommand(createUserDTO));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<GetUserDTO>> LoginUser([FromBody] LoginUserDTO loginUserDTO)
        {
            return await Mediator.Send(new LoginUserCommand(loginUserDTO));
        }

        [HttpGet("list"), Authorize(Roles = AuthRoles.Admin)]
        public async Task<ActionResult<PaginatedApiResult<GetUserDTO>>> ListUsers([FromQuery] PaginationQuery paginationQuery)
        {
            return await Mediator.Send(new ListUsersQuery(paginationQuery.PageIndex, paginationQuery.PageSize));
        }
    }
}
