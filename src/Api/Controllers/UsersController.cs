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
    /// <summary>
    /// Users Controller
    /// </summary>
    public class UsersController : BaseApiController
    {
        /// <summary>
        /// Creates a User
        /// </summary>
        /// <remarks>
        /// <b>Implementation Notes</b> 
        /// <br/><br />
        /// Only users with the role <b>Admin</b> can create a user
        /// </remarks>
        /// <param name="createUserDTO">
        /// Takes <b>FirstName</b>, <b>LastName</b>, <b>Email</b>, <b>Password</b> and <b>Role</b>
        /// <br/>
        /// All fields are mandatory
        /// <br/><br/>
        /// <b>Available Role Options</b>:
        /// <br/>
        /// Member
        /// <br/>
        /// Admin
        /// </param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<GetUserDTO>> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            return await Mediator.Send(new CreateUserCommand(createUserDTO));
        }

        /// <summary>
        /// Generates a User's Access Token
        /// </summary>
        /// <remarks>
        /// <b>Implementation Notes</b> 
        /// <br/><br />
        /// All Users can generate their Token
        /// </remarks>
        /// <param name="loginUserDTO">
        /// Takes <b>Email</b> and <b>Password</b>
        /// <br/>
        /// All fields are mandatory
        /// </param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("generateToken")]
        public async Task<ActionResult<GetUserDTO>> LoginUser([FromBody] LoginUserDTO loginUserDTO)
        {
            return await Mediator.Send(new LoginUserCommand(loginUserDTO));
        }

        /// <summary>
        /// Lists all Users
        /// </summary>
        /// <remarks>
        /// <b>Implementation Notes</b> 
        /// <br/><br />
        /// Only users with the role<b> Admin</b> can list all users
        /// <br/>
        /// Pagination is by <b>Page Index</b> and <b>Page Size</b> 
        /// </remarks>
        /// <param name="paginationQuery">
        /// Pagination is by <b>Page Index</b> and <b>Page Size</b> 
        /// </param>
        /// <returns></returns>
        [HttpGet("list"), Authorize(Roles = AuthRoles.Admin)]
        public async Task<ActionResult<PaginatedApiResult<GetUserDTO>>> ListUsers([FromQuery] PaginationQuery paginationQuery)
        {
            return await Mediator.Send(new ListUsersQuery(paginationQuery.PageIndex, paginationQuery.PageSize));
        }
    }
}
