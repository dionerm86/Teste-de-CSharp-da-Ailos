using MediatR;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Questao5.Application.Commands.Movimentacao;
using Questao5.Application.Helpers;

namespace Questao5.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public MovementsController(IMediator mediator)
        {
            _mediator = mediator;
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt));

            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Cria um novo movimento.
        /// </summary>
        /// <param name="command">Dados do comando para criar o movimento.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMovement([FromBody] CriarMovimentacaoCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _retryPolicy.ExecuteAsync(() =>
                    _circuitBreakerPolicy.ExecuteAsync(() =>
                        _mediator.Send(command)));

                return CreatedAtAction(nameof(CreateMovement), new { id = result }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao criar o movimento", Details = ex.Message });
            }
        }

        [HttpGet("{idContaCorrente}/saldo")]
        [ProducesResponseType(typeof(Result<long>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSaldo(string idContaCorrente)
        {
            var query = new ConsultarSaldoCommand(idContaCorrente);
            var result = await _mediator.Send(query);

            if (!result.IsValid)
                return BadRequest(new { mensagem = result.ErrorMessage, tipo = result.ErrorType });

            return Ok(new
            {
                Numero = result.Data.Numero,
                NomeTitular = result.Data.Nome,
                DataHoraResposta = result.Data.DataHoraResposta,
                SaldoAtual = result.Data.SaldoAtual
            });
        }
    }
}

