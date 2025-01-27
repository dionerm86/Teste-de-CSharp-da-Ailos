using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;

namespace Questao5.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovementsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria um novo movimento.
        /// </summary>
        /// <param name="command">Dados do comando para criar o movimento.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMovement([FromBody] CreateMovementCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(CreateMovement), new { id = result }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao criar o movimento", Details = ex.Message });
            }
        }
    }
}

