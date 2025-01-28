using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces;

public interface IIdempotenciaRepositorio
{
    Task CriarIdempotencia(Idempotencia idempotencia);
    Task<Idempotencia> IsExisteChaveIdempotente(string idContaCorrente);
}
