using MediatR;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Helpers;

namespace Questao5.Application.Commands.Movimentacao
{
    public class ConsultarSaldoCommand : IRequest<Result<SaldoResponse>>
    {
        public Guid IdContaCorrente { get; }

        public ConsultarSaldoCommand(Guid idContaCorrente)
        {
            IdContaCorrente = idContaCorrente;
        }
    }

}
