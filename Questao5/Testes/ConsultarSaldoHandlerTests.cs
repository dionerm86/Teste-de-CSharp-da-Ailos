using FluentAssertions;
using NSubstitute;
using Questao5.Application.Commands.ConsultaSaldo.Handler;
using Questao5.Application.Commands.Movimentacao;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Questao5.Testes.Mocks;
using Xunit;

namespace Questao5.Testes;

public class ConsultarSaldoHandlerTests
{
    private readonly IContaCorrenteRepositorio _contaCorrenteRepositorio;
    private readonly IMovimentacaoRepositorio _movimentacaoRepositorio;
    private readonly ConsultarSaldoHandler _handler;

    public ConsultarSaldoHandlerTests()
    {
        _contaCorrenteRepositorio = Substitute.For<IContaCorrenteRepositorio>();
        _movimentacaoRepositorio = Substitute.For<IMovimentacaoRepositorio>();
        _handler = new ConsultarSaldoHandler(_contaCorrenteRepositorio, _movimentacaoRepositorio);
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_ContaNaoEncontrada()
    {
        // Arrange
        var command = new ConsultarSaldoCommand("invalid-id");

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns((ContaCorrente)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Conta corrente não encontrada.");
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_ContaInativa()
    {
        // Arrange
        var command = ConsultarSaldoCommandMock.Get("inactive-id");

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente, 0);

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns(contaCorrente);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Conta corrente está inativa.");
    }

    [Fact]
    public async Task Handle_Deve_Retornar_Saldo_Com_Sucesso()
    {
        // Arrange
        var command = new ConsultarSaldoCommand("valid-id");

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente);
        var saldoAtual = 1000m;

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns(contaCorrente);
        _movimentacaoRepositorio.ObterSaldoAtualAsync(command.IdContaCorrente).Returns(saldoAtual);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.SaldoAtual.Should().Be(1000m);
    }
}