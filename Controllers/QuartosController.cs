using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOTEL360___Trabalho_final.Data;
using HOTEL360___Trabalho_final.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;

namespace HOTEL360___Trabalho_final.Controllers{

    [Authorize] // qualquer tarefa desta classe só pode ser efetuada por pessoas autorizadas (ie. autenticadas)
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

        /// <summary>
        /// mostra todos os quartos existentes na BD
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous] // esta anotação isenta da obrigação do utilizador estar autenticado
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
        public async Task<IActionResult> Create([Bind("Id,Nome,Capacidade,Preco,PrecoAux,Descricao,Imagem")] Quartos quarto, IFormFile ImagemLogo) {

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
                                
                try
                {
                    //transferir o valor de PrecoAux para Preco
                    quarto.Preco = Convert.ToDecimal(quarto.PrecoAux.Replace('.', ','));

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
                catch (Exception ex)
                {
                    // se cheguei aqui é pq aconteceu um problema
                    // crítico. TEM de ser tratado.
                    //    - devolver o controlo ao utilizador
                    //    - corrigir o erro
                    //    - escrever os dados do erro num LOG
                    //    - escrever os dados do erro numa tabela da BD
                    //    - etc.
                    throw;
                }            

            
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

            var quarto = await _context.Quartos.FindAsync(id);
            if (quarto == null) {
                return NotFound();
            }

            // Preencher o valor de PrecoAux para exibição
            quarto.PrecoAux = quarto.Preco.ToString();

            string nomeImagem = quarto.Imagem;

            return View(quarto);
        }

        // POST: Quartos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Capacidade,Preco,PrecoAux,Descricao,Imagem")] Quartos quarto, IFormFile ImagemLogo) {
            if (id != quarto.Id)  {
                return NotFound();
            }
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
                try {

                    //transferir o valor de PrecoAux para Preco
                    quarto.Preco = Convert.ToDecimal(quarto.PrecoAux.Replace('.', ','));

                    //atualiza os dados vindos da View à BD 
                    _context.Update(quarto);
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
                catch (DbUpdateConcurrencyException)   {
                    if (!QuartosExists(quarto.Id)) {
                        return NotFound();
                    }
                    else   {
                        throw;
                    }
                }
            }
            return View(quarto);
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

            // Nas ações de DELETE também é crucial a existência
            // de Try Catch 

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
