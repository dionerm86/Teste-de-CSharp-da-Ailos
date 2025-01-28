using FluentAssertions;
using NSubstitute;
using Questao5.Application.Commands.Movimentacao.Handler;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Questao5.Testes.Mocks;
using Xunit;

namespace Questao5.Testes;
public class CriarMovimentacaoHandlerTests
{
    private readonly IContaCorrenteRepositorio _contaCorrenteRepositorio;
    private readonly IMovimentacaoRepositorio _movimentacaoRepositorio;
    private readonly IIdempotenciaRepositorio _ideaRepositorio;
    private readonly CriarMovimentacaoHandler _handler;

    public CriarMovimentacaoHandlerTests()
    {
        _contaCorrenteRepositorio = Substitute.For<IContaCorrenteRepositorio>();
        _movimentacaoRepositorio = Substitute.For<IMovimentacaoRepositorio>();
        _ideaRepositorio = Substitute.For<IIdempotenciaRepositorio>();

        _handler = new CriarMovimentacaoHandler(_contaCorrenteRepositorio, _ideaRepositorio, _movimentacaoRepositorio);
    }

    [Fact]
    public async Task Handle_Deve_Criar_Movimentacao_Tipo_Credito_Com_Sucesso()
    {
        // Arrange
        var command = CriarMovimentoCommandMock.Get("C");

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente);

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns(contaCorrente);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeTrue();
        await _movimentacaoRepositorio.Received(1).CriarMovimentacao(Arg.Is<Movimento>(m =>
            m.IdContaCorrente == command.IdContaCorrente &&
            Math.Abs(m.Valor - command.Valor) < 0.01m &&
            m.TipoMovimento == "C"
        ));
    }

    [Fact]
    public async Task Handle_Deve_Criar_Movimentacao_Tipo_Debito_Com_Sucesso()
    {
        // Arrange
        var command = CriarMovimentoCommandMock.Get("D");

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente);

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns(contaCorrente);

        var saldoAtual = command.Valor * 2;

        _movimentacaoRepositorio.ObterSaldoAtualAsync(command.IdContaCorrente).Returns(saldoAtual);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeTrue();
        await _movimentacaoRepositorio.Received(1).CriarMovimentacao(Arg.Is<Movimento>(m =>
            m.IdContaCorrente == command.IdContaCorrente &&
            Math.Abs(m.Valor - command.Valor) < 0.01m &&  // Tolerância para valores decimais
            m.TipoMovimento == "D"
        ));

    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_ChaveIdempotente_Ja_Existir()
    {
        // Arrange
        var command = CriarMovimentoCommandMock.Get("C");

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente);

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns(contaCorrente);

        var saldoAtual = command.Valor * 2;

        _movimentacaoRepositorio.ObterSaldoAtualAsync(command.IdContaCorrente).Returns(saldoAtual);
        _ = _ideaRepositorio.IsExisteChaveIdempotente(command.ChaveIdempotencia.ToString()).Returns(new Idempotencia
        {
            ChaveIdempotencia = command.ChaveIdempotencia.ToString(),
            Requisicao = "FakeResultado",
            Resultado = "FakeResultado"
        });

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Já existe uma movimentação com a chave de idempotencia fornecida.");
        await _movimentacaoRepositorio.DidNotReceive().CriarMovimentacao(Arg.Any<Movimento>());
    }

    [Fact]
    public async Task Handle_Deve_Retornar_Falha_Quando_ContaCorrente_Nao_Existir()
    {
        // Arrange
        var command = CriarMovimentoCommandMock.Get("C");

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente, 0);

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns((ContaCorrente)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Conta corrente não encontrada.");
        await _movimentacaoRepositorio.DidNotReceive().CriarMovimentacao(Arg.Any<Movimento>());
    }

    [Fact]
    public async Task Handle_Deve_Retornar_Falha_Quando_Conta_For_Inativa()
    {
        // Arrange
        var command = CriarMovimentoCommandMock.Get("C");

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente, 0);

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns(contaCorrente);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Conta corrente inativa.");
        await _movimentacaoRepositorio.DidNotReceive().CriarMovimentacao(Arg.Any<Movimento>());
    }

    [Fact]
    public async Task Handle_Deve_Retornar_Falha_Quando_Saldo_For_Insuficiente_Para_Debito()
    {
        // Arrange
        var command = CriarMovimentoCommandMock.Get("D");

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente);

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns(contaCorrente);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Saldo insuficiente para realizar a operação.");
        await _movimentacaoRepositorio.DidNotReceive().CriarMovimentacao(Arg.Any<Movimento>());
    }

    [Theory]
    [InlineData("C")]
    [InlineData("D")]
    public async Task Handle_Deve_Retornar_Falha_Quando_Valor_Da_Movimentacao_For_Negativo(string tipoMovimento)
    {
        // Arrange
        var command = CriarMovimentoCommandMock.Get(tipoMovimento);

        command.Valor = -1;

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente);

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns(contaCorrente);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("O valor deve ser positivo.");
        await _movimentacaoRepositorio.DidNotReceive().CriarMovimentacao(Arg.Any<Movimento>());
    }

    [Fact]
    public async Task Handle_Deve_Retornar_Falha_Quando_TipoMovimento_For_Inválido()
    {
        // Arrange
        var command = CriarMovimentoCommandMock.Get("A");

        var contaCorrente = ContaCorrenteMock.Get(command.IdContaCorrente);

        _contaCorrenteRepositorio.ObterPorId(command.IdContaCorrente).Returns(contaCorrente);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Tipo de movimento inválido.");
        await _movimentacaoRepositorio.DidNotReceive().CriarMovimentacao(Arg.Any<Movimento>());
    }
}

