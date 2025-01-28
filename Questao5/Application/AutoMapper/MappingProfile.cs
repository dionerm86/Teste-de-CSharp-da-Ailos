using AutoMapper;
using Questao5.Application.Commands.Movimentacao;
using Questao5.Application.DTOs.Request;

namespace Questao5.Application.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CriarMovimentoRequest, CriarMovimentoCommand>();
    }
}
