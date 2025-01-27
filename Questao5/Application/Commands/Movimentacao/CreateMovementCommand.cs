using MediatR;

namespace Questao5.Application.Commands.Movimentacao
{
    public class CreateMovementCommand : IRequest<string>
    {
        public string IdContaCorrente { get; set; }
        public string Numero { get; set; }
        public decimal Valor { get; set; }
        public string TipoMovimentacao { get; set; } // "C" or "D"
        public Guid ChaveIdempotencia { get; set; }
    }
}
