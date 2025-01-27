using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface IMovimentacaoRepositorio
    {
        Task CriarMovimentacao(Movimento movimento);
        Task<decimal> ObterSaldoAtualAsync(string idContaCorrente);

    }
}
