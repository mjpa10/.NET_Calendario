using APICalendario.Models;
using AutoMapper;

namespace APICalendario.DTOs.Mappings;

public class LembreteDTOMappingProfile : Profile
{
    public LembreteDTOMappingProfile()
    {
        CreateMap<Lembrete, LembreteDTO>().ReverseMap();
    }  
}
