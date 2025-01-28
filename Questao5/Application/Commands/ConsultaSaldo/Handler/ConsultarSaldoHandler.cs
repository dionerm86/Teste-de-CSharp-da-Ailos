using MediatR;
using Questao5.Application.Commands.Movimentacao;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Helpers;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Commands.ConsultaSaldo.Handler
{
    public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoCommand, Result<SaldoResponse>>
    {
        private readonly IContaCorrenteRepositorio _contaCorrenteRepositorio;
        private readonly IMovimentacaoRepositorio _movimentacaoRepositorio;

        public ConsultarSaldoHandler(
            IContaCorrenteRepositorio contaCorrenteRepositorio,
            IMovimentacaoRepositorio movimentacaoRepositorio)
        {
            _contaCorrenteRepositorio = contaCorrenteRepositorio;
            _movimentacaoRepositorio = movimentacaoRepositorio;
        }

        public async Task<Result<SaldoResponse>> Handle(ConsultarSaldoCommand request, CancellationToken cancellationToken)
        {
            var contaCorrente = await _contaCorrenteRepositorio.ObterPorId(request.IdContaCorrente);
            if (contaCorrente == null)
                return Result<SaldoResponse>.Failure("Conta corrente não encontrada.", "INVALID_ACCOUNT");
            
            if (!Convert.ToBoolean(contaCorrente.Ativo))
                return Result<SaldoResponse>.Failure("Conta corrente está inativa.", "INACTIVE_ACCOUNT");

            var saldo = await _movimentacaoRepositorio.ObterSaldoAtualAsync(request.IdContaCorrente);

            var response = new SaldoResponse
            {
                Numero = contaCorrente.Numero,
                Nome = contaCorrente.Nome,
                DataHoraResposta = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                SaldoAtual = decimal.Parse(saldo.ToString("N2"))
            };

            return Result<SaldoResponse>.Success(response);
        }
    }

}
