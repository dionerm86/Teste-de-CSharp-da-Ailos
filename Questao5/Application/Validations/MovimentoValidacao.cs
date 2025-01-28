using Questao5.Application.Commands.Movimentacao;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Helpers;

namespace Questao5.Application.Validations
{
    public static class MovimentoValidacao
    {
        public static Result<CriarMovimentoResponse> ValidarMovimentacao(CriarMovimentacaoCommand request, int ativo, decimal saldoAtual)
        {
            if (request == null)
                return Result<CriarMovimentoResponse>.Failure("A requisição é nula.", "StatusCode 400: ");

            if (ativo == 0)
                return Result<CriarMovimentoResponse>.Failure("Conta corrente inativa.", "INACTIVE_ACCOUNT: ");

            if (request.Valor <= 0)
                return Result<CriarMovimentoResponse>.Failure("O valor deve ser positivo.", "INVALID_VALUE: ");

            if (request.TipoMovimentacao != "C" && request.TipoMovimentacao != "D")
                return Result<CriarMovimentoResponse>.Failure("Tipo de movimento inválido.", "INVALID_TYPE: ");

            if (request.TipoMovimentacao == "D" && request.Valor > saldoAtual)
                return Result<CriarMovimentoResponse>.Failure("Saldo insuficiente para realizar a operação.", "INSUFFICIENT_FUNDS: ");

            return Result<CriarMovimentoResponse>.Success(new CriarMovimentoResponse());
        }
    }
}
