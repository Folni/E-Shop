using AutoMapper;
using WebAPI.Models; 
using WebApplication.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Proizvod, ProizvodDTO>()
            .ForMember(dest => dest.KategorijaNaziv,
                       opt => opt.MapFrom(src => src.Kategorija.Naziv))
            .ForMember(dest => dest.DrzaveNazivi,
                       opt => opt.MapFrom(src => src.Drzavas.Select(d => d.Naziv).ToList()));
    }
}