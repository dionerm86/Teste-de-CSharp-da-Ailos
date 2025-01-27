using Questao5.Application.Commands.Movimentacao;

namespace Questao5.Application.Validations
{
    public static class MovimentoValidacao
    {
        public static void ValidarMovimentacao(CriarMovimentacaoCommand request, int ativo, decimal saldoAtual)
        {
            if (request == null)
                throw new Exception("400");

            if (ativo == 0)
                throw new Exception("INACTIVE_ACCOUNT: Conta corrente inativa.");

            if (request.Valor <= 0)
                throw new Exception("INVALID_VALUE: O valor deve ser positivo.");

            if (request.TipoMovimentacao != "C" && request.TipoMovimentacao != "D")
                throw new Exception("INVALID_TYPE: Tipo de movimento inválido.");

            if (request.TipoMovimentacao == "D")
            {
                if (request.Valor > saldoAtual)
                {
                    throw new Exception("INSUFFICIENT_FUNDS: Saldo insuficiente para realizar a operação.");
                }
            }
        }
    }
}
