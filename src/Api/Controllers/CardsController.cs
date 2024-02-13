﻿using Api.Common;
using Application.Cards.Commands;
using Application.Cards.Commands.DTOs;
using Application.Cards.Queries.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class CardsController : BaseApiController
    {
        [HttpPost("create")]
        public async Task<ActionResult<GetCardDTO>> CreateCard([FromBody] CreateCardDTO createCardDTO)
        {
            return await Mediator.Send(new CreateCardCommand(createCardDTO));
        }

        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<GetCardDTO>> GetCards([FromRoute] Guid id, [FromBody] UpdateCardDTO updateCardDTO)
        {
            return await Mediator.Send(new UpdateCardCommand(id, updateCardDTO));
        }
    }
}
