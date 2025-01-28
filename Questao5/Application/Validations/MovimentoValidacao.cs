using Questao5.Application.Commands.Movimentacao;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Helpers;

namespace Questao5.Application.Validations
{
    public static class MovimentoValidacao
    {
        public static Result<CriarMovimentoResponse> ValidarMovimentacao(CriarMovimentoCommand request, int ativo, decimal saldoAtual)
        {
            if (request == null)
                return Result<CriarMovimentoResponse>.Failure("A requisição é nula.", "StatusCode 400: ");

            if (ativo == 0)
                return Result<CriarMovimentoResponse>.Failure("Conta corrente inativa.", "INACTIVE_ACCOUNT: ");

            if (request.Valor <= 0)
                return Result<CriarMovimentoResponse>.Failure("O valor deve ser positivo.", "INVALID_VALUE: ");

            if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
                return Result<CriarMovimentoResponse>.Failure("Tipo de movimento inválido.", "INVALID_TYPE: ");

            if (request.TipoMovimento == "D" && decimal.Parse(request.Valor.ToString("N2")) > decimal.Parse(saldoAtual.ToString("N2")))
                return Result<CriarMovimentoResponse>.Failure("Saldo insuficiente para realizar a operação.", "INSUFFICIENT_FUNDS: ");

            return Result<CriarMovimentoResponse>.Success(new CriarMovimentoResponse());
        }
    }
}
