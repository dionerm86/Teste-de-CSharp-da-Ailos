using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Questao5.Api.Controllers;
using Questao5.Application.Commands.Movimentacao;
using Questao5.Application.Commands.Responses;
using Questao5.Application.DTOs.Request;
using Questao5.Application.Helpers;
using Xunit;

namespace Questao5.Testes;

public class MovementsControllerTests
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly MovimentoController _controller;

    public MovementsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _mapper = Substitute.For<IMapper>();
        _controller = new MovimentoController(_mediator, _mapper);
    }

    [Fact]
    public async Task CreateMovement_Deve_Retornar201_Com_Sucesso()
    {
        // Arrange
        var request = new CriarMovimentoRequest
        {
            IdContaCorrente = "valid-id",
            Valor = 100m,
            TipoMovimento = "C",
            ChaveIdempotencia = Guid.NewGuid()
        };

        var command = new CriarMovimentoCommand
        {
            IdContaCorrente = request.IdContaCorrente,
            Valor = request.Valor,
            TipoMovimento = request.TipoMovimento,
            ChaveIdempotencia = request.ChaveIdempotencia
        };

        var response = Result<CriarMovimentoResponse>.Success(new CriarMovimentoResponse());

        _mapper.Map<CriarMovimentoCommand>(request).Returns(command);
        _mediator.Send(command).Returns(response);

        // Act
        var result = await _controller.CreateMovement(request) as CreatedAtActionResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task CriarMovimento_Deve_Retornar500_Quando_Exception()
    {
        // Arrange
        var request = new CriarMovimentoRequest
        {
            IdContaCorrente = "valid-id",
            Valor = 100m,
            TipoMovimento = "C",
            ChaveIdempotencia = Guid.NewGuid()
        };

        _mapper.Map<CriarMovimentoCommand>(request).Returns(new CriarMovimentoCommand());
        _mediator.Send(Arg.Any<CriarMovimentoCommand>()).Returns<Task>(_ => throw new Exception("Fake Error"));

        // Act
        var result = await _controller.CreateMovement(request) as ObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(500);
        result.Value.Should().BeEquivalentTo(new { Message = "Erro ao criar o movimento", Details = "Fake Error" });
    }

    [Fact]
    public async Task GetSaldo_Deve_Retornar200_Com_Sucesso()
    {
        // Arrange
        var idContaCorrente = "valid-id";

        var response = Result<SaldoResponse>.Success(new SaldoResponse
        {
            Numero = 123,
            Nome = "João",
            SaldoAtual = 1000m,
            DataHoraResposta = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")
        });

        _mediator.Send(Arg.Any<ConsultarSaldoCommand>()).Returns(response);

        // Act
        var result = await _controller.GetSaldo(idContaCorrente) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(new
        {
            Numero = 123,
            Nome = "João",
            SaldoAtual = 1000m,
            DataHoraResposta = response.Data.DataHoraResposta
        });
    }

    [Fact]
    public async Task GetSaldo_Deve_Retornar400_Quando_ContaNaoEncontrada()
    {
        // Arrange
        var idContaCorrente = "invalid-id";

        var response = Result<SaldoResponse>.Failure("Conta corrente não encontrada.", "INVALID_ACCOUNT");

        _mediator.Send(Arg.Any<ConsultarSaldoCommand>()).Returns(response);

        // Act
        var result = await _controller.GetSaldo(idContaCorrente) as BadRequestObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
    }
}

