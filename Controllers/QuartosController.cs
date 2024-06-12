﻿using System;
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

        /// <summary>
        /// referência à BD do projeto
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// objeto que contém os dados referentes ao ambiente 
        /// do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QuartosController(
         ApplicationDbContext context,
         IWebHostEnvironment webHostEnvironment) {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

            /* Algoritmo
          * 1- há ficheiro?
          *    1.1 - não há ficheiro!
          *          devolver à view dizendo que o ficheiro
          *          é obrigatório
          *    1.2 - há ficheiro!
          *          Mas, é uma imagem (PNG, JPG)?
          *          1.2.1 - não é PNG nem JPG
          *                  devolver o controlo à View
          *                  e pedir PNG ou JPG
          *          1.2.2 - é uma imagem
          *                  - determinar o nome a atribuir 
          *                    ao ficheiro
          *                  - escrever esse nome na BD
          *                  - se a escrita na BD se concretizar
          *                    é que o ficheiro é guardado no 
          *                    disco rígido
          */

            // vars. auxiliares
            string nomeImagem = "";
            bool haImagem = false;

            // há ficheiro?
            if (ImagemLogo == null)
            {
                ModelState.AddModelError("",
                   "O fornecimento de uma Imagem é obrigatório!");
                return View(quarto);
            }
            else
            {
                // há ficheiro, mas é imagem?
                if (!(ImagemLogo.ContentType == "image/png" ||
                       ImagemLogo.ContentType == "image/jpeg")
                   )
                {
                    ModelState.AddModelError("",
                   "Tem de fornecer para a Imagem um ficheiro PNG ou JPG!");
                    return View(quarto);
                }
                else
                {
                    // há ficheiro, e é uma imagem válida
                    haImagem = true;
                    // obter o nome a atribuir à imagem
                    Guid g = Guid.NewGuid();
                    nomeImagem = g.ToString();
                    // obter a extensão do nome do ficheiro
                    string extensao = Path.GetExtension(ImagemLogo.FileName);
                    // adicionar a extensão ao nome da imagem
                    nomeImagem += extensao;
                    // adicionar o nome do ficheiro ao objeto que
                    // vem do browser
                    quarto.Imagem = nomeImagem;
                }
            }

            //avalia se os dados que chegam da View estão de acordo com o Model
            if (ModelState.IsValid) {

                //adiciona os dados vindos da View à BD 
                _context.Add(quarto);
                //efetua COMMIT na BD
                await _context.SaveChangesAsync();

                // se há ficheiro de imagem,
                // vamos guardar no disco rígido do servidor
                if (haImagem)
                {
                    // determinar onde se vai guardar a imagem
                    string nomePastaOndeGuardarImagem =
                       _webHostEnvironment.WebRootPath;
                    // já sei o caminho até à pasta wwwroot
                    // especifico onde vou guardar a imagem
                    nomePastaOndeGuardarImagem =
                       Path.Combine(nomePastaOndeGuardarImagem, "Imagens");
                    // e, existe a pasta 'Imagens'?
                    if (!Directory.Exists(nomePastaOndeGuardarImagem))
                    {
                        Directory.CreateDirectory(nomePastaOndeGuardarImagem);
                    }
                    // juntar o nome do ficheiro à sua localização
                    string nomeFinalDaImagem =
                       Path.Combine(nomePastaOndeGuardarImagem, nomeImagem);

                    // guardar a imagem no disco rigído
                    using var stream = new FileStream(
                       nomeFinalDaImagem, FileMode.Create
                       );
                    await ImagemLogo.CopyToAsync(stream);
                }


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
