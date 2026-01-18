using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers 
{
    [Authorize]
    public class ProizvodiController : Controller
    {
        private readonly EtrgovinaContext _context;

        public ProizvodiController(EtrgovinaContext context)
        {
            _context = context;
        }

        // GET: /Proizvodi/Index
        public async Task<IActionResult> Index()
        {
            var proizvodi = await _context.Proizvods
                .Include(p => p.Kategorija)
                .Include(p => p.Drzavas)
                .ToListAsync();
            return View(proizvodi);
        }

        // GET: /Proizvodi/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.KategorijaSelect = await _context.Kategorijas
                .Select(k => new SelectListItem
                {
                    Value = k.KategorijaId.ToString(),
                    Text = k.Naziv
                }).ToListAsync();

            ViewBag.DrzaveLista = await _context.Drzavas
                .Select(d => new SelectListItem
                {
                    Value = d.DrzavaId.ToString(),
                    Text = d.Naziv
                }).ToListAsync();

            return View(new ProizvodVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProizvodVM model)
        {
            if (ModelState.IsValid)
            {
                var noviProizvod = new Proizvod
                {
                    Naziv = model.Naziv,
                    Cijena = model.Cijena,
                    Opis = model.Opis,
                    KategorijaId = model.KategorijaId
                };

                if (model.OdabraneDrzaveIds != null && model.OdabraneDrzaveIds.Any())
                {
                    foreach (var drzavaId in model.OdabraneDrzaveIds)
                    {
                        var drzava = await _context.Drzavas.FindAsync(drzavaId);
                        if (drzava != null)
                        {
                            noviProizvod.Drzavas.Add(drzava);
                        }
                    }
                }

                _context.Proizvods.Add(noviProizvod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await NapuniViewBagove();
            return View(model);
        }

        // GET: /Proizvodi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var proizvod = await _context.Proizvods
                .Include(p => p.Drzavas)
                .FirstOrDefaultAsync(p => p.ProizvodId == id);

            if (proizvod == null) return NotFound();

            var model = new ProizvodVM
            {
                Id = proizvod.ProizvodId,
                Naziv = proizvod.Naziv,
                Cijena = proizvod.Cijena,
                Opis = proizvod.Opis,
                KategorijaId = proizvod.KategorijaId,
                OdabraneDrzaveIds = proizvod.Drzavas.Select(d => d.DrzavaId).ToList()
            };

            await NapuniViewBagove();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProizvodVM model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var proizvod = await _context.Proizvods
                        .Include(p => p.Drzavas)
                        .FirstOrDefaultAsync(p => p.ProizvodId == id);

                    if (proizvod == null) return NotFound();

                    proizvod.Naziv = model.Naziv;
                    proizvod.Cijena = model.Cijena;
                    proizvod.Opis = model.Opis;
                    proizvod.KategorijaId = model.KategorijaId;

                    proizvod.Drzavas.Clear(); 
                    if (model.OdabraneDrzaveIds != null)
                    {
                        foreach (var drzavaId in model.OdabraneDrzaveIds)
                        {
                            var drzava = await _context.Drzavas.FindAsync(drzavaId);
                            if (drzava != null) proizvod.Drzavas.Add(drzava);
                        }
                    }

                    _context.Update(proizvod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Proizvods.Any(e => e.ProizvodId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await NapuniViewBagove();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var proizvod = await _context.Proizvods.FindAsync(id);
            if (proizvod != null)
            {
                _context.Proizvods.Remove(proizvod);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Proizvod '{proizvod.Naziv}' je uspješno obrisan.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task NapuniViewBagove()
        {
            ViewBag.KategorijaSelect = await _context.Kategorijas
                .Select(k => new SelectListItem { Value = k.KategorijaId.ToString(), Text = k.Naziv })
                .ToListAsync();

            ViewBag.DrzaveLista = await _context.Drzavas
                .Select(d => new SelectListItem { Value = d.DrzavaId.ToString(), Text = d.Naziv })
                .ToListAsync();
        }
    }
}