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
using Microsoft.AspNetCore.Identity;

namespace HOTEL360___Trabalho_final.Controllers{

    /* apenas as pessoas autenticadas E que pertençam 
     * ao Role de GERENTE podem entrar 
     */
    [Authorize(Roles = "Gerentes, Hospedes")] 
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        ///  Objeto para interagir com a Autenticação
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;

        public ReservasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)  {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservas
        /// <summary>
        /// mostra todas as reservas existentes na BD
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var listaRsvs = _context.Reservas.Include(r => r.Quarto);
            return View(await listaRsvs.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }


            // procura os dados da Reserva
            // em SQL : SELECT *
            //          FROM Reservas rs INNER JOIN Quartos q ON rs.QuartoFK = q.Id
            //                                       INNER JOIN ReservasServicos ress ON ress.ReservaFK = rs.ID
            //                                       INNER JOIN Servicos s ON ress.ServFK = s.Id
            //          WHERE rs.Id = id 

            // em LINQ : 
            var reservas = await _context.Reservas
                .Include(r => r.Quarto)
                .Include(r => r.ListaServicos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservas == null)   {
                return NotFound();
            }

            return View(reservas);
        }

        // GET: Reservas/Create
        public IActionResult Create()  {
            // efetuar uma pesquisa na BD pelos Quartos
            // que podem estar associados à FK Quartos
            // SelectList -> cria uma lista de 'options' para a dropdown
            // Expressão LINQ para efetuar a pesquisa dos Quartos
            //    _context.Quartos.OrderBy(c=>c.Nome)
            ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome");

            // Obter a lista de serviços,
            // para enviar para a View
            // em SQL: SELECT * FROM Servicos s ORDER BY s.Nome
            // em LINQ:
            var listaSer = _context.Servicos.OrderBy(p => p.Nome).ToList();
            ViewData["listaServicos"] = listaSer;

            //devolve controlo à View
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// recolha de dados na adição de uma nova Reserva
        /// </summary>
        /// <param name="reserva">dados da Reserva</param>
        /// <param name="escolhaServicos">lista dos IDs dos serviços que
        ///         ficarão associados à Reserva</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ValorPago,ValorPagoAux,DataReserva,DataCheckIN,DataCheckOUT,QuartoFK")] Reservas reserva, int[] escolhaServicos)  {

            // var. auxiliar
            bool haErros = false;

            //Validações
            if (reserva.QuartoFK == -1)  {
                ModelState.AddModelError("", "Escolha um quarto, por favor.");
                haErros = true;
            }

            if (escolhaServicos.Length == 0)
            {
                // não escolhi nenhum serviço
                ModelState.AddModelError("", "Escolha um serviço, pf.");
                haErros = true;
            }



            if (ModelState.IsValid && !haErros) {

                try  {

                    // associar os serviços escolhidos à Reserva
                    // criar uma Lista de serviços
                    var listaServicosNaRes = new List<Servicos>();
                    foreach (var serv in escolhaServicos) {
                        // procurar o Serviço na BD
                        var s = await _context.Servicos.FindAsync(serv);
                        if (s != null) {
                            listaServicosNaRes.Add(s);
                        }
                    }
                    // atribuir a lista de serviços à Reserva
                    reserva.ListaServicos = listaServicosNaRes;


                    //transferir o valor de VAlorPagoAux para ValorPago
                    reserva.ValorPago = Convert.ToDecimal(reserva.ValorPagoAux.Replace('.', ','));

                    //adiciona os dados vindos da View à BD
                    _context.Add(reserva);
                    //efetua COMMIT na BD
                    await _context.SaveChangesAsync();

                    //redireciona o utilizador para a página Index
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)  {
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
            // Se chego aqui é pq alguma coisa correu mal
            // Vou devolver o controlo à View
            // Tenho de preparar os dados a enviar
            ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Id), "Id", "Id", reserva.QuartoFK);

            var listaSer = _context.Servicos.OrderBy(p => p.Nome).ToList();
            ViewData["listaServicos"] = listaSer;


            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservas = await _context.Reservas.FindAsync(id);
            if (reservas == null)
            {
                return NotFound();
            }
            ViewData["QuartoFK"] = new SelectList(_context.Quartos, "Id", "Id", reservas.QuartoFK);
            return View(reservas);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ValorPago,DataReserva,DataCheckIN,DataCheckOUT,QuartoFK")] Reservas reservas)
        {
            if (id != reservas.Id)  {
                return NotFound();
            }

            if (ModelState.IsValid)   {
                try
                {
                    _context.Update(reservas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)  {
                    if (!ReservasExists(reservas.Id)) {
                        return NotFound();
                    }
                    else  {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuartoFK"] = new SelectList(_context.Quartos, "Id", "Id", reservas.QuartoFK);
            return View(reservas);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)  {
            if (id == null)   {
                return NotFound();
            }

            var reservas = await _context.Reservas
                .Include(r => r.Quarto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservas == null)   {
                return NotFound();
            }

            return View(reservas);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            // Nas ações de DELETE também é crucial a existência
            // de Try Catch 

            var reservas = await _context.Reservas.FindAsync(id);
            if (reservas != null)  {
                _context.Reservas.Remove(reservas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservasExists(int id)  {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
