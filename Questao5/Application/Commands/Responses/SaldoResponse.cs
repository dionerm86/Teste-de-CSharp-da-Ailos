namespace Questao5.Application.Commands.Responses
{
    public class SaldoResponse
    {
        public int Numero { get; set; }
        public string Nome { get; set; }
        public DateTime DataHoraResposta { get; set; }
        public decimal SaldoAtual { get; set; }
    }
}
