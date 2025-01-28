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

namespace Questao5.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimentoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly IMapper _mapper;

    public MovimentoController(IMediator mediator, IMapper mapper)
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
    [SwaggerRequestExample(typeof(CriarMovimentoRequest), typeof(CriarMovimentoRequestExemplo))]
    [ProducesResponseType(typeof(Result<CriarMovimentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateMovement([FromBody] CriarMovimentoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var command = _mapper.Map<CriarMovimentoCommand>(request);

            var response = await _retryPolicy.ExecuteAsync(() =>
                _circuitBreakerPolicy.ExecuteAsync(() =>
                    _mediator.Send(command)));

            if (response.IsValid)
                return CreatedAtAction(nameof(CreateMovement), response.Data);
            else
                return BadRequest(response);
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

        var result = await _retryPolicy.ExecuteAsync(() =>
                _circuitBreakerPolicy.ExecuteAsync(() =>
                    _mediator.Send(new ConsultarSaldoCommand(idContaCorrente))));

        if (!result.IsValid)
            return BadRequest(result);
        
        return Ok(result.Data);
    }
}

