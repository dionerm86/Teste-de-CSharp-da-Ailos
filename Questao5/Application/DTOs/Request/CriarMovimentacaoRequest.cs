namespace Questao5.Application.DTOs.Request;

public class CriarMovimentacaoRequest
{
    public string IdContaCorrente { get; set; }
    public string Numero { get; set; }
    public decimal Valor { get; set; }
    public string TipoMovimentacao { get; set; }
    public Guid ChaveIdempotencia { get; set; }
}
