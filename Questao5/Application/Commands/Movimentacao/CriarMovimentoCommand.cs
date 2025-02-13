﻿using MediatR;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Helpers;

namespace Questao5.Application.Commands.Movimentacao;
public class CriarMovimentoCommand : IRequest<Result<CriarMovimentoResponse>>
{
    public string IdContaCorrente { get; set; }
    public string Numero { get; set; }
    public decimal Valor { get; set; }
    public string TipoMovimento { get; set; } // "C" or "D"
    public Guid ChaveIdempotencia { get; set; }
}

