using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly EtrgovinaContext _context;

        public HomeController(EtrgovinaContext context)
        {
            _context = context;
        }

        // GET: /Home/Index (Popis proizvoda s pretraživanjem)
        public async Task<IActionResult> Index(string searchString)
        {
            var upit = _context.Proizvods
                .Include(p => p.Kategorija)
                .Include(p => p.Drzavas)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                upit = upit.Where(s => s.Naziv.Contains(searchString) || s.Opis.Contains(searchString));
            }

            return View(await upit.ToListAsync());
        }
    }
}
