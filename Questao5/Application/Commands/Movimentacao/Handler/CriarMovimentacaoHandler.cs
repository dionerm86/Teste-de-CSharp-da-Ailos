using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Helpers;
using Questao5.Application.Validations;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Commands.Movimentacao.Handler;

public class CriarMovimentacaoHandler : IRequestHandler<CriarMovimentoCommand, Result<CriarMovimentoResponse>>
{
    private readonly IContaCorrenteRepositorio _contaCorrenteRepositorio;
    private readonly IIdempotenciaRepositorio _idempotenciaRepositorio;
    private readonly IMovimentacaoRepositorio _movimentacaoRepositorio;

    public CriarMovimentacaoHandler(IContaCorrenteRepositorio contaCorrenteRepositorio, IIdempotenciaRepositorio idempotenciaRepositorio, IMovimentacaoRepositorio movimentacaoRepositorio)
    {
        _contaCorrenteRepositorio = contaCorrenteRepositorio;
        _idempotenciaRepositorio = idempotenciaRepositorio;
        _movimentacaoRepositorio = movimentacaoRepositorio;
    }

    public async Task<Result<CriarMovimentoResponse>> Handle(CriarMovimentoCommand request, CancellationToken cancellationToken)
    {
        var contaCorrente = await _contaCorrenteRepositorio.ObterPorId(request.IdContaCorrente);

        if (contaCorrente == null)
            return Result<CriarMovimentoResponse>.Failure("Conta corrente não encontrada.", "INVALID_ACCOUNT: ");

        var saldoAtual = await _movimentacaoRepositorio.ObterSaldoAtualAsync(request.IdContaCorrente);

        var validacao = MovimentoValidacao.ValidarMovimentacao(request, contaCorrente.Ativo, saldoAtual);

        if (!validacao.IsValid)
            return validacao;

        var existingRequest = await _idempotenciaRepositorio.IsExisteChaveIdempotente(request.ChaveIdempotencia.ToString());

        if (existingRequest != null)
            return Result<CriarMovimentoResponse>.Failure("Já existe uma movimentação com a chave de idempotencia fornecida.", "INVALID_ACTION:");

        var requestJson = JsonConvert.SerializeObject(request);

        var movimento = new Movimento
        {
            IdMovimento = Guid.NewGuid().ToString(),
            IdContaCorrente = request.IdContaCorrente,
            DataMovimento = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            TipoMovimento = request.TipoMovimento,
            Valor = decimal.Parse(request.Valor.ToString("N2"))
        };

        await _movimentacaoRepositorio.CriarMovimentacao(movimento);

        var resultJson = JsonConvert.SerializeObject(new { movimento.IdMovimento });

        var idempotencia = new Idempotencia
        {
            ChaveIdempotencia = request.ChaveIdempotencia.ToString(),
            Requisicao = requestJson,
            Resultado = resultJson
        };

        await _idempotenciaRepositorio.CriarIdempotencia(idempotencia);

        return Result<CriarMovimentoResponse>.Success(new CriarMovimentoResponse { IdMovimento = movimento.IdMovimento });
    }
}
