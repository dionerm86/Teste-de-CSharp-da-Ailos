using Questao5.Application.Commands.Movimentacao;
using Questao5.Application.DTOs.Request;
using Swashbuckle.AspNetCore.Filters;

namespace Questao5.Api.Examples;

public class CriarMovimentoRequestExemplo : IExamplesProvider<CriarMovimentoRequest>
{
    public CriarMovimentoRequest GetExamples()
    {
        return new CriarMovimentoRequest
        {
            IdContaCorrente = Guid.NewGuid().ToString(),
            ChaveIdempotencia = Guid.NewGuid(),
            Numero = "123",
            TipoMovimento = "C",
            Valor = 1500
        };
    }
}
