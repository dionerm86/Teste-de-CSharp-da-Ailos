using Questao5.Application.Commands.Movimentacao;
using Questao5.Application.DTOs.Request;
using Swashbuckle.AspNetCore.Filters;

namespace Questao5.Api.Examples
{

    public class CriarMovimentacaoRequestExemplo : IExamplesProvider<CriarMovimentacaoRequest>
    {
        public CriarMovimentacaoRequest GetExamples()
        {
            return new CriarMovimentacaoRequest
            {
                IdContaCorrente = Guid.NewGuid().ToString(),
                ChaveIdempotencia = Guid.NewGuid(),
                Numero = "123",
                TipoMovimentacao = "C",
                Valor = 1500
            };
        }
    }
}
