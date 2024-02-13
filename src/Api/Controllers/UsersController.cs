using Api.Common;
using Application.Users.Commands;
using Application.Users.Commands.DTOs;
using Application.Users.Queries.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class UsersController : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<GetUserDTO>> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            return await Mediator.Send(new CreateUserCommand(createUserDTO));
        }

        [HttpPost("login")]
        public async Task<ActionResult<GetUserDTO>> LoginUser([FromBody] LoginUserDTO loginUserDTO)
        {
            return await Mediator.Send(new LoginUserCommand(loginUserDTO));
        }
    }
}
