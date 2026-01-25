using Microsoft.AspNetCore.Mvc;
using ETrgovina.DAL.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using WebApplication.DTOs;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly EtrgovinaContext _context;
        private readonly IMapper _mapper;

        public HomeController(EtrgovinaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: /Home/Index
        public async Task<IActionResult> Index(string searchString, int pg = 1)
        {
            const int pageSize = 6; 

            var upit = _context.Proizvods
                .Include(p => p.Kategorija)
                .Include(p => p.Drzavas)
                .AsQueryable();

            // Filtriranje po pretrazi
            if (!string.IsNullOrEmpty(searchString))
            {
                upit = upit.Where(s => s.Naziv.Contains(searchString) || s.Opis.Contains(searchString));
            }

            // Izračun podataka za straničenje
            var ukupnoProizvoda = await upit.CountAsync();

            // Logika za preskakanje (Skip) i uzimanje (Take) određenog broja zapisa
            var proizvodiEntiteti = await upit
                .Skip((pg - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Mapiranje u DTO
            var modelZaPrikaz = _mapper.Map<List<ProizvodDTO>>(proizvodiEntiteti);

            ViewBag.CurrentPage = pg;
            ViewBag.TotalPages = (int)Math.Ceiling((double)ukupnoProizvoda / pageSize);
            ViewBag.SearchString = searchString;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProizvodiPartial", modelZaPrikaz);
            }

            return View(modelZaPrikaz);
        }
    }
}