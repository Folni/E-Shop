using AutoMapper;
using ETrgovina.DAL.Models;
using WebAPI.DTO;
using WebAPI.View;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // --- PROIZVOD ---
        CreateMap<Proizvod, ProizvodDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProizvodId))
            .ForMember(dest => dest.KategorijaNaziv, opt => opt.MapFrom(src => src.Kategorija.Naziv))
            .ReverseMap()
            .ForMember(dest => dest.ProizvodId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Kategorija, opt => opt.Ignore());

        CreateMap<Proizvod, ProizvodView>()
            .ForMember(dest => dest.KategorijaNaziv, opt => opt.MapFrom(src => src.Kategorija.Naziv));

        // --- KATEGORIJA I DRŽAVA ---
        CreateMap<Kategorija, KategorijaDTO>().ReverseMap();
        CreateMap<Drzava, DrzavaDTO>().ReverseMap();

        // --- KORISNIK I AUTH ---
        CreateMap<Korisnik, KorisnikView>();
        CreateMap<Korisnik, KorisnikDTO>().ReverseMap();

        CreateMap<UserRegisterDTO, Korisnik>()
            .ForMember(dest => dest.LozinkaHash, opt => opt.Ignore());

        CreateMap<Korisnik, AuthResponseDTO>()
            .ForMember(dest => dest.ImePrezime, opt => opt.MapFrom(src => $"{src.Ime} {src.Prezime}"))
            .ForMember(dest => dest.Token, opt => opt.Ignore());

        // --- NARUDŽBA ---
        CreateMap<Narudzba, NarudzbaView>()
            .ForMember(dest => dest.KorisnikIme, opt => opt.MapFrom(src => $"{src.Korisnik.Ime} {src.Korisnik.Prezime}"))
            .ForMember(dest => dest.Proizvodi, opt => opt.MapFrom(src => src.NarudzbaProizvods));

        CreateMap<NarudzbaProizvod, NarudzbaProizvodView>()
            .ForMember(dest => dest.NazivProizvoda, opt => opt.MapFrom(src => src.Proizvod.Naziv))
            .ForMember(dest => dest.Cijena, opt => opt.MapFrom(src => src.Proizvod.Cijena))
            .ForMember(dest => dest.Ukupno, opt => opt.MapFrom(src => (src.Kolicina ?? 0) * src.Proizvod.Cijena));

        CreateMap<NarudzbaDTO, Narudzba>()
            .ForMember(dest => dest.NarudzbaProizvods, opt => opt.MapFrom(src => src.Stavke));

        CreateMap<StavkaDTO, NarudzbaProizvod>();

        // --- LOG ---
        CreateMap<Logovi, LogDTO>().ReverseMap();
    }
}