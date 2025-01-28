namespace Questao5.Application.DTOs.Request;

public class CriarMovimentoRequest
{
    public string IdContaCorrente { get; set; }
    public string Numero { get; set; }
    public decimal Valor { get; set; }
    public string TipoMovimento { get; set; }
    public Guid ChaveIdempotencia { get; set; }
}
