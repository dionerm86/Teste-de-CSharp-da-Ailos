using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Validations;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Commands.Movimentacao.Handler
{
    public class CriarMovimentacaoHandler : IRequestHandler<CriarMovimentacaoCommand, string>
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

        public async Task<string> Handle(CriarMovimentacaoCommand request, CancellationToken cancellationToken)
        {
            var contaCorrente = await _contaCorrenteRepositorio.ObterPorId(request.IdContaCorrente) ??
                throw new Exception("INVALID_ACCOUNT: Conta corrente não encontrada.");

            var saldoAtual = await _movimentacaoRepositorio.ObterSaldoAtualAsync(request.IdContaCorrente);

            MovimentoValidacao.ValidarMovimentacao(request, contaCorrente.Ativo, saldoAtual);

            var existingRequest = await _idempotenciaRepositorio.IsExisteChaveIdempotente(request.ChaveIdempotencia.ToString());

            if (existingRequest != null)
                return existingRequest.Resultado;

            var requestJson = JsonConvert.SerializeObject(request);

            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                TipoMovimento = request.TipoMovimentacao,
                Valor = request.Valor
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

            return movimento.IdMovimento;
        }
    }
}
