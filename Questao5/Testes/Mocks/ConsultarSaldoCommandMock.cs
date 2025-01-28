using Questao5.Application.Commands.Movimentacao;

namespace Questao5.Testes.Mocks;

public static class ConsultarSaldoCommandMock
{
    public static ConsultarSaldoCommand Get(string idContaCorrente)
    {
        return new ConsultarSaldoCommand(idContaCorrente);
    }
}
