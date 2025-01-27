using MediatR;
using Newtonsoft.Json;
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
            // Validations
            var contaCorrente = await _contaCorrenteRepositorio.ObterPorId(request.IdContaCorrente);

            if (contaCorrente == null)
                throw new Exception("INVALID_ACCOUNT: Conta corrente não encontrada.");

            if (contaCorrente.Ativo == 0)
                throw new Exception("INACTIVE_ACCOUNT: Conta corrente inativa.");

            if (request.Valor <= 0)
                throw new Exception("INVALID_VALUE: O valor deve ser positivo.");

            if (request.TipoMovimentacao != "C" && request.TipoMovimentacao != "D")
                throw new Exception("INVALID_TYPE: Tipo de movimento inválido.");

            // Idempotency check
            var existingRequest = await _idempotenciaRepositorio.IsExisteChaveIdempotente(request.ChaveIdempotencia.ToString());

            if (existingRequest != null)
                return existingRequest.Resultado;

            // Serialize the request to JSON
            var requestJson = JsonConvert.SerializeObject(request);

            // Create the movement
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
