using Bogus;
using Questao5.Application.Commands.Movimentacao;

namespace Questao5.Testes.Mocks;

public static class CriarMovimentoCommandMock
{
    public static CriarMovimentoCommand Get(string tipoMovimento)
    {
        var faker = new Faker();
        return new CriarMovimentoCommand
        {
            IdContaCorrente = Guid.NewGuid().ToString(),
            ChaveIdempotencia = Guid.NewGuid(),
            Numero = faker.Random.Int(100, 999).ToString(),
            TipoMovimento = tipoMovimento,
            Valor = faker.Random.Decimal(100, 1000)
        };
    }
}
