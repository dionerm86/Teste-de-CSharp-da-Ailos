using MediatR;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Helpers;

namespace Questao5.Application.Commands.Movimentacao;

public class ConsultarSaldoCommand : IRequest<Result<SaldoResponse>>
{
    public string IdContaCorrente { get; }

    public ConsultarSaldoCommand(string idContaCorrente)
    {
        IdContaCorrente = idContaCorrente;
    }
}
