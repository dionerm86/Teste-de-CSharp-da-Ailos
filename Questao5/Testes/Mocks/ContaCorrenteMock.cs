using Bogus;
using Questao5.Domain.Entities;

namespace Questao5.Testes.Mocks;

public static class ContaCorrenteMock
{
    public static ContaCorrente Get(string idContaCorrente, int ativo = 1)
    {
        var faker = new Faker();

        return new ContaCorrente
        {
            IdContaCorrente = idContaCorrente,
            Numero = faker.Random.Int(1, 999),
            Nome = faker.Name.ToString(),
            Ativo = ativo
        };
    }
}
