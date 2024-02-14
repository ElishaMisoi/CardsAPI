using Api.Common;
using Application.Cards.Commands;
using Application.Cards.Commands.DTOs;
using Application.Cards.Queries;
using Application.Cards.Queries.DTOs;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// Cards Controller
    /// </summary>
    public class CardsController : BaseApiController
    {
        /// <summary>
        /// Creates a Card
        /// </summary>
        /// <remarks>
        /// <b>Implementation Notes</b> 
        /// <br/><br />
        /// All users can create a Card (users with the roles <b>Admin</b> and <b>Member</b>)
        /// </remarks>
        /// <param name="createCardDTO"><b>Name</b> is mandatory, <b>Description</b>  and <b>Color</b> are optional
        /// <br/>
        /// <b>Color</b>, if provided, should conform to a “6 alphanumeric characters prefixed with a #“ format e.g <b>#000000</b>
        /// </param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult<GetCardDTO>> CreateCard([FromBody] CreateCardDTO createCardDTO)
        {
            return await Mediator.Send(new CreateCardCommand(createCardDTO));
        }

        /// <summary>
        /// Gets a Card by ID
        /// </summary>
        /// <remarks>
        /// <b>Implementation Notes</b> 
        /// <br/><br />
        /// Users with the role <b>Member</b> can only get their own Cards
        /// <br/>
        /// Users with the role <b>Admin</b> can get all Cards
        /// </remarks>
        /// <param name="id">The <b>ID</b> of the card to be fetched</param>
        /// <returns></returns>
        [HttpGet("get/{id:guid}")]
        public async Task<ActionResult<GetCardDTO>> GetCardById([FromRoute] Guid id)
        {
            return await Mediator.Send(new GetCardByIdCommand(id));
        }

        /// <summary>
        /// Filters, Paginates and Lists Cards
        /// </summary>
        /// <remarks>
        /// <b>Implementation Notes</b> 
        /// <br/><br />
        /// Users with the role <b>Member</b> can only view their own Cards
        /// <br/>
        /// Users with the role <b>Admin</b> can view all Cards
        /// <br/><br/>
        /// Filters include <b>Name</b>, <b>Color</b>, <b>Status</b> and <b>Date</b>
        /// <br/>
        /// Results can be sorted by <b>Name</b>, <b>Color</b>, <b>Status</b> and <b>Date</b>
        /// <br/>
        /// Pagination is by <b>Page Index</b> and <b>Page Size</b> 
        /// <br/><br/>
        /// <b>Available Status Options</b>:
        /// <br/>
        /// ToDo
        /// <br/>
        /// InProgress
        /// <br/>
        /// Done
        /// <br/><br/>
        /// <b>Available Sort Options</b>:
        /// <br/>
        /// Name
        /// <br/>
        /// Color
        /// <br/>
        /// Status
        /// <br/>
        /// Date
        /// </remarks>
        /// <param name="searchCardDTO">
        /// Filters include <b>Name</b>, <b>Color</b>, <b>Status</b> and <b>Date</b>
        /// <br/>
        /// Results can be sorted by <b>Name</b>, <b>Color</b>, <b>Status</b> and <b>Date</b>
        /// <br/>
        /// Pagination is by <b>Page Index</b> and <b>Page Size</b> 
        /// <br/><br/>
        /// <b>Available Status Options</b>:
        /// <br/>
        /// ToDo
        /// <br/>
        /// InProgress
        /// <br/>
        /// Done
        /// <br/><br/>
        /// <b>Available Sort Options</b>:
        /// <br/>
        /// Name
        /// <br/>
        /// Color
        /// <br/>
        /// Status
        /// <br/>
        /// Date
        /// </param>
        /// <returns></returns>
        [HttpGet("list"), Authorize(Roles = AuthRoles.Admin)]
        public async Task<ActionResult<PaginatedApiResult<GetCardDTO>>> ListCards([FromQuery] SearchCardDTO searchCardDTO)
        {
            return await Mediator.Send(new ListCardsQuery(searchCardDTO));
        }

        /// <summary>
        /// Updates a Card
        /// </summary>
        /// <remarks>
        /// <b>Implementation Notes</b> 
        /// <br/><br />
        /// Users with the role <b>Member</b> can only update their own Cards
        /// <br/>
        /// Users with the role <b>Admin</b> can update all Cards
        /// </remarks>
        /// <param name="id">The <b>ID</b> of the Card to be updated</param>
        /// <param name="updateCardDTO">Takes the <b>Name</b>, <b>Description</b>, <b>Color</b> and <b>Status</b> of Card
        /// <br/> <br/>
        /// <b>Available Status Options</b>:
        /// <br/>
        /// ToDo
        /// <br/>
        /// InProgress
        /// <br/>
        /// Done
        /// </param>
        /// <returns></returns>
        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<GetCardDTO>> UpdateCard([FromRoute] Guid id, [FromBody] UpdateCardDTO updateCardDTO)
        {
            return await Mediator.Send(new UpdateCardCommand(id, updateCardDTO));
        }

        /// <summary>
        /// Deletes a Card
        /// </summary>
        /// <remarks>
        /// <b>Implementation Notes</b> 
        /// <br/><br />
        /// Users with the role <b>Member</b> can only delete their own Cards
        /// <br/>
        /// Users with the role <b>Admin</b> can delete all Cards
        /// </remarks>
        /// <param name="id">The <b>ID</b> of the card to be deleted</param>
        /// <returns></returns>
        [HttpDelete("delete/{id:guid}")]
        public async Task<ActionResult> DeleteCard([FromRoute] Guid id)
        {
            return await Mediator.Send(new DeleteCardCommand(id));
        }
    }
}