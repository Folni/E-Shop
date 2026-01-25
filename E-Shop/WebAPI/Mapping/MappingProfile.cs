using AutoMapper;
using ETrgovina.DAL.Models;
using WebAPI.DTO;
using WebAPI.View;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // --- PROIZVOD ---
        // Mapiranje za listanje (View) i detalje (DTO)
        CreateMap<Proizvod, ProizvodView>()
            .ForMember(dest => dest.KategorijaNaziv, opt => opt.MapFrom(src => src.Kategorija.Naziv));

        CreateMap<Proizvod, ProizvodDTO>()
            .ForMember(dest => dest.KategorijaNaziv, opt => opt.MapFrom(src => src.Kategorija.Naziv))
            .ReverseMap(); // ReverseMap omogućuje da se ProizvodDTO vrati nazad u Proizvod kod unosa

        // --- KATEGORIJA I DRŽAVA ---
        CreateMap<Kategorija, KategorijaDTO>().ReverseMap();
        CreateMap<Drzava, DrzavaDTO>().ReverseMap();

        // --- KORISNIK I AUTH ---
        CreateMap<Korisnik, KorisnikView>();
        CreateMap<Korisnik, KorisnikDTO>().ReverseMap();

        // Mapiranje za registraciju (DTO -> Model)
        // Lozinku obično ne mapiramo direktno jer ide kroz hashing
        CreateMap<UserRegisterDTO, Korisnik>()
            .ForMember(dest => dest.LozinkaHash, opt => opt.Ignore());

        // Posebno mapiranje za AuthResponse
        CreateMap<Korisnik, AuthResponseDTO>()
            .ForMember(dest => dest.ImePrezime, opt => opt.MapFrom(src => $"{src.Ime} {src.Prezime}"))
            .ForMember(dest => dest.Token, opt => opt.Ignore()); // Token se ručno postavlja u kontroleru

        // --- NARUDŽBA ---
        // Model -> View (za GET)
        CreateMap<Narudzba, NarudzbaView>()
            .ForMember(dest => dest.KorisnikIme, opt => opt.MapFrom(src => $"{src.Korisnik.Ime} {src.Korisnik.Prezime}"))
            .ForMember(dest => dest.Proizvodi, opt => opt.MapFrom(src => src.NarudzbaProizvods));

        // Model stavke -> View stavke
        CreateMap<NarudzbaProizvod, NarudzbaProizvodView>()
            .ForMember(dest => dest.NazivProizvoda, opt => opt.MapFrom(src => src.Proizvod.Naziv))
            .ForMember(dest => dest.Cijena, opt => opt.MapFrom(src => src.Proizvod.Cijena))
            .ForMember(dest => dest.Ukupno, opt => opt.MapFrom(src => (src.Kolicina ?? 0) * src.Proizvod.Cijena));

        // DTO -> Model (za POST - kreiranje narudžbe)
        CreateMap<NarudzbaDTO, Narudzba>()
            .ForMember(dest => dest.NarudzbaProizvods, opt => opt.MapFrom(src => src.Stavke));

        CreateMap<StavkaDTO, NarudzbaProizvod>();

        // --- LOG ---
        CreateMap<Logovi, LogDTO>().ReverseMap();
    }
}