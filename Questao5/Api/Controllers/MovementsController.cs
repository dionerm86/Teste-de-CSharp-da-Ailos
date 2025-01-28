using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Questao5.Api.Examples;
using Questao5.Application.Commands.Movimentacao;
using Questao5.Application.Commands.Responses;
using Questao5.Application.DTOs.Request;
using Questao5.Application.Helpers;
using Swashbuckle.AspNetCore.Filters;

namespace Questao5.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly IMapper _mapper;

        public MovementsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, tentar => TimeSpan.FromSeconds(tentar));

            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
            _mapper = mapper;
        }

        /// <summary>
        /// Cria um novo movimento.
        /// </summary>
        /// <param name="request">Dados da requisição para criar o movimento.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost]
        [SwaggerRequestExample(typeof(CriarMovimentacaoRequest), typeof(CriarMovimentacaoRequestExemplo))]
        [ProducesResponseType(typeof(Result<CriarMovimentoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMovement([FromBody] CriarMovimentacaoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var command = _mapper.Map<CriarMovimentacaoCommand>(request);

                var response = await _retryPolicy.ExecuteAsync(() =>
                    _circuitBreakerPolicy.ExecuteAsync(() =>
                        _mediator.Send(command)));

                return CreatedAtAction(nameof(CreateMovement), response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao criar o movimento", Details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o saldo atual do cliente.
        /// </summary>
        /// <param name="idContaCorrente">Id da conta corrente do cliente.</param>
        /// <returns>Saldo atual do cliente</returns>
        [HttpGet("{idContaCorrente}/saldo")]
        [ProducesResponseType(typeof(Result<long>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSaldo(string idContaCorrente)
        {
            var result = await _mediator.Send(new ConsultarSaldoCommand(idContaCorrente));

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

