using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface IMovimentacaoRepositorio
    {
        Task<decimal> GetTotalCreditosAsync(Guid idContaCorrente);
        Task<decimal> GetTotalDebitosAsync(Guid idContaCorrente);
        Task CriarMovimentacao(Movimento movimento);

    }
}
