namespace Questao5.Application.Commands.Responses
{
    public class SaldoResponse
    {
        public string Numero { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataHoraResposta { get; set; }
        public decimal SaldoAtual { get; set; }
    }
}
