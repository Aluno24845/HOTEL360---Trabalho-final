using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOTEL360___Trabalho_final.Data;
using HOTEL360___Trabalho_final.Models;

namespace HOTEL360___Trabalho_final.Controllers{
    public class QuartosController : Controller {

        private readonly ApplicationDbContext _context;

        public QuartosController(ApplicationDbContext context) {
            _context = context;
        }

        // GET: Quartos
        public async Task<IActionResult> Index() {
            return View(await _context.Quartos.ToListAsync());
        }

        // GET: Quartos/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var quartos = await _context.Quartos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quartos == null) {
                return NotFound();
            }

            return View(quartos);
        }

        // GET: Quartos/Create
        [HttpGet] // facultativo, pois esta função,
                  // por predefinição, já reage ao HTTP GET
        public IActionResult Create() {
            // a única ação desta função é mostrar a View quando quero iniciar a adição de um Quarto
            return View();
        }

        // POST: Quartos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Capacidade,Preco,Descricao,Imagem")] Quartos quarto, IFormFile ImagemLogo) {
            
            //avalia se os dados que chegam da View estão de acordo com o Model
            if (ModelState.IsValid) {

                //adiciona os dados vindos da View à BD 
                _context.Add(quarto);
                //efetua COMMIT na BD
                await _context.SaveChangesAsync();
                //redireciona o utilizador para a página Index 
                return RedirectToAction(nameof(Index));
            }

            //se chegou aqui é porque algo não correu bem
            //volta à View com os dados fornecidos pela View 
            return View(quarto);
        }

        // GET: Quartos/Edit/5
        public async Task<IActionResult> Edit(int? id)  {
            if (id == null) {
                return NotFound();
            }

            var quartos = await _context.Quartos.FindAsync(id);
            if (quartos == null) {
                return NotFound();
            }
            return View(quartos);
        }

        // POST: Quartos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Capacidade,Preco,Descricao,Imagem")] Quartos quartos) {
            if (id != quartos.Id)  {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(quartos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)   {
                    if (!QuartosExists(quartos.Id)) {
                        return NotFound();
                    }
                    else   {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(quartos);
        }

        // GET: Quartos/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var quartos = await _context.Quartos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quartos == null) {
                return NotFound();
            }

            return View(quartos);
        }

        // POST: Quartos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var quartos = await _context.Quartos.FindAsync(id);
            if (quartos != null) {
                _context.Quartos.Remove(quartos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuartosExists(int id) {
            return _context.Quartos.Any(e => e.Id == id);
        }
    }
}
