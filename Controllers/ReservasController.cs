﻿using System;
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
using System.Security.Claims;
using System.Globalization;

namespace HOTEL360___Trabalho_final.Controllers
{

    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        ///  Objeto para interagir com a Autenticação
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;

        public ReservasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservas
        /// <summary>
        /// mostra todas as reserva existentes na BD
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {


            if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
            {
                return View(await _context.Reservas
                .Include(r => r.Quarto)
                .Include(r => r.Hospede) // Incluir o Hospede na consulta
                .ToListAsync());
            }
            else
            {

                return View(await _context.Reservas
                    .Include(r => r.Quarto)
                    .Include(r => r.Hospede) // Incluir o Hospede na consulta
                    .Where(predicate: r => r.Hospede.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)) // apenas as reservas do utilizador autenticado
                    .ToListAsync());
            }

        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
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
                .Include(r => r.Hospede) // Incluir o Hospede na consulta
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservas == null)
            {
                return NotFound();
            };

            return View(reservas);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
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

            /*
             * Aceder à lista de Hospedes se a pessoa que interage
             * é do Role GERENTES ou RECCECIONISTAS
             */
            if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
            {
                // efetuar uma pesquisa na BD pelos Hospedes
                // que podem estar associados à FK Hospedes
                // SelectList -> cria uma lista de 'options' para a dropdown
                // Expressão LINQ para efetuar a pesquisa dos Quartos
                //    _context.Hosdes.OrderBy(c=>c.Nome)
                ViewData["HospedeId"] = new SelectList(_context.Hospedes.OrderBy(q => q.Nome), "Id", "Nome");

            }

            var lista = new List<object>();

            // Obter a lista de quartos incluindo a lista de reservas
            var listaQuartos = _context.Quartos
                .OrderBy(q => q.Nome)
                .Include(q => q.ListaReservas)
                .ToList();

            // Percorrer a lista de quartos
            foreach (var quarto in listaQuartos)
            {

                var obj = new
                {
                    Id = quarto.Id,
                    Nome = quarto.Nome,
                    Preco = quarto.Preco,
                    Reservas = new List<object>()
                };

                foreach (var reserva in quarto.ListaReservas)
                {
                    var objReserva = new
                    {
                        Id = reserva.Id,
                        DataCheckIN = reserva.DataCheckIN,
                        DataCheckOUT = reserva.DataCheckOUT
                    };

                    obj.Reservas.Add(objReserva);
                }
                lista.Add(obj);

            }

            // Enviar a lista de quartos para a View
            ViewData["Quartos"] = lista;


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
        public async Task<IActionResult> Create([Bind("Id,ValorPago,ValorPagoAux,DataCheckIN,DataCheckOUT,QuartoFK, HospedeId")] Reservas reserva, int[] escolhaServicos)
        {

            // var. auxiliar
            bool haErros = false;

            // Define a Data da Reserva como a data atual
            reserva.DataReserva = DateTime.Now;

            // No caso do Utilizador ser do role GERENTES ou RECCECIONISTA 
            // Verifica se escolheu um hospede para associar à Reserva
            if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
            {

                //Validações
                if (reserva.HospedeId == -1)
                {
                    ModelState.AddModelError("", "Escolha um hospede, por favor.");
                    haErros = true;
                }
            }
            else
            {
                // Caso seja um Hospede então associa o seu Id à Reserva
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var utilizador = await _context.Utilizadores
                        .FirstOrDefaultAsync(m => m.UserId == user);
                reserva.HospedeId = utilizador.Id;
            }

            //Validações
            if (reserva.QuartoFK == -1)
            {
                ModelState.AddModelError("", "Escolha um quarto, por favor.");
                haErros = true;
            }

            if (escolhaServicos.Length == 0)
            {
                // não escolhi nenhum serviço
                ModelState.AddModelError("", "Escolha um serviço, por favor.");
                haErros = true;
            }



            if (ModelState.IsValid && !haErros)
            {

                try
                {

                    // Verifica se a data de check-in é inferior à data da reserva
                    if (reserva.DataCheckIN < reserva.DataReserva)
                    {
                        ModelState.AddModelError(nameof(reserva.DataCheckIN), "A data de check-in não pode ser anterior à data da reserva.");

                        // Recarregar as listas e devolve o controlo à View
                        ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome", reserva.QuartoFK);
                        var listaServicos = _context.Servicos.OrderBy(p => p.Nome).ToList();
                        ViewData["listaServicos"] = listaServicos;

                        if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
                        {
                            ViewData["HospedeId"] = new SelectList(_context.Hospedes.OrderBy(q => q.Nome), "Id", "Nome", reserva.HospedeId);
                        }

                        return View(reserva);
                    }

                    // Verifica se o check-out é pelo menos um dia após o check-in
                    if (reserva.DataCheckIN >= reserva.DataCheckOUT)
                    {
                        ModelState.AddModelError(nameof(reserva.DataCheckOUT), "A data de check-out deve ser após a data de check-in.");

                        // Recarregar as listas e devolve o controlo à View
                        ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome", reserva.QuartoFK);
                        var listaServicos = _context.Servicos.OrderBy(p => p.Nome).ToList();
                        ViewData["listaServicos"] = listaServicos;

                        if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
                        {
                            ViewData["HospedeId"] = new SelectList(_context.Hospedes.OrderBy(q => q.Nome), "Id", "Nome", reserva.HospedeId);
                        }

                        return View(reserva);
                    }

                    // Verifica se o check-out ocorre em um dia diferente do check-in
                    if (reserva.DataCheckIN.Date == reserva.DataCheckOUT.Date)
                    {
                        ModelState.AddModelError(nameof(reserva.DataCheckOUT), "A estadia deve incluir pelo menos uma noite.");

                        // Recarregar as listas e devolve o controlo à View
                        ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome", reserva.QuartoFK);
                        var listaServicos = _context.Servicos.OrderBy(p => p.Nome).ToList();
                        ViewData["listaServicos"] = listaServicos;

                        if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
                        {
                            ViewData["HospedeId"] = new SelectList(_context.Hospedes.OrderBy(q => q.Nome), "Id", "Nome", reserva.HospedeId);
                        }

                        return View(reserva);
                    }


                    // associar os serviços escolhidos à Reserva
                    // criar uma Lista de serviços
                    var listaServicosNaRes = new List<Servicos>();
                    foreach (var serv in escolhaServicos)
                    {
                        // procurar o Serviço na BD
                        var s = await _context.Servicos.FindAsync(serv);
                        if (s != null)
                        {
                            listaServicosNaRes.Add(s);
                        }
                    }
                    // atribuir a lista de serviços à Reserva
                    reserva.ListaServicos = listaServicosNaRes;


                    //transferir o valor de VAlorPagoAux para ValorPago
                    reserva.ValorPago = Convert.ToDecimal(reserva.ValorPagoAux.Replace('.', ','));

                    // Calcula o valor total
                    reserva.ValorTotal = CalcularValorTotal(reserva);

                    // Calcula o valor que ainda falta pagar
                    reserva.ValorAPagar = reserva.ValorTotal - reserva.ValorPago;

                    //Verifica se o Valor Pago é superior ao Valor Total
                    if (reserva.ValorPago > reserva.ValorTotal)
                    {
                        ModelState.AddModelError("", "O Valor Pago não pode ser superior ao valor Total!");

                        // Recarregar as listas e devolve o controlo à View
                        ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome", reserva.QuartoFK);
                        var listaServicos = _context.Servicos.OrderBy(p => p.Nome).ToList();
                        ViewData["listaServicos"] = listaServicos;

                        if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
                        {
                            ViewData["HospedeId"] = new SelectList(_context.Hospedes.OrderBy(q => q.Nome), "Id", "Nome", reserva.HospedeId);
                        }

                        return View(reserva);
                    }


                    //adiciona os dados vindos da View à BD
                    _context.Add(reserva);
                    //efetua COMMIT na BD
                    await _context.SaveChangesAsync();

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
            // Se chego aqui é pq alguma coisa correu mal
            // Vou devolver o controlo à View
            // Tenho de preparar os dados a enviar
            ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome", reserva.QuartoFK);

            var listaSer = _context.Servicos.OrderBy(p => p.Nome).ToList();
            ViewData["listaServicos"] = listaSer;

            /*
             * Aceder à lista de Hospedes se a pessoa que interage
             * é do Role GERENTES ou RECCECIONISTAS
             */
            if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
            {
                ViewData["HospedeId"] = new SelectList(_context.Hospedes.OrderBy(q => q.Nome), "Id", "Nome");

            }


            return View(reserva);
        }

        /* apenas as pessoas autenticadas E que pertençam 
         * ao Role de GERENTES ou Role de RECCECIONISTA podem entrar */
        [Authorize(Roles = "Gerentes, Reccecionistas")]
        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Recuperar a reserva existente, incluindo os serviços associados
            var reserva = await _context.Reservas
                .Include(r => r.ListaServicos)
                .Include(r => r.Hospede) // Incluir o Hospede na consulta
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // Preencher o valor de ValorPagoAux para exibição
            reserva.ValorPagoAux = reserva.ValorPago.ToString();

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
            //var listaSer = _context.Servicos.OrderBy(p => p.Nome).ToList();
            //ViewData["listaServicos"] = listaSer;
            var listaSer = _context.Servicos.OrderBy(p => p.Nome).ToList();
            ViewData["listaServicos"] = listaSer;
            ViewData["servicosSelecionados"] = reserva.ListaServicos.Select(s => s.Id).ToList();


            // TODO - melhorar informacao enviada - enviar apenas, preco, id, none
            //ViewData["Quartos"] = _context.Quartos.OrderBy(q => q.Nome).ToArray();
            var lista = new List<object>();

            var listaQuartos = _context.Quartos
                .OrderBy(q => q.Nome)
                .Include(q => q.ListaReservas)
                .ToList();

            foreach (var quarto in listaQuartos)
            {

                var obj = new
                {
                    Id = quarto.Id,
                    Nome = quarto.Nome,
                    Preco = quarto.Preco,
                    Reservas = new List<object>()
                };

                foreach (var reservaQuarto in quarto.ListaReservas)
                {
                    var objReserva = new
                    {
                        Id = reservaQuarto.Id,
                        DataCheckIN = reservaQuarto.DataCheckIN,
                        DataCheckOUT = reservaQuarto.DataCheckOUT
                    };

                    obj.Reservas.Add(objReserva);
                }
                lista.Add(obj);

            }

            ViewData["Quartos"] = lista;
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ValorPago,DataReserva, ValorPagoAux,DataCheckIN,DataCheckOUT, QuartoFK, HospedeId")] Reservas reserva, int[] escolhaServicos)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }
            var reservaGuardada = await _context.Reservas
                .Include(r => r.ListaServicos)
                .Include(r => r.Hospede) // Incluir o Hospede na consulta
                .FirstOrDefaultAsync(r => r.Id == id);

            //var reservaGuardada = await _context.Reservas.FindAsync(id);

            if (reservaGuardada == null)
            {
                return NotFound();
            }

            if (reserva.QuartoFK > 0)
            {
                reservaGuardada.QuartoFK = reserva.QuartoFK;
            }
            reserva.HospedeId = reservaGuardada.HospedeId;
            reservaGuardada.DataCheckIN = reserva.DataCheckIN;
            reservaGuardada.DataCheckOUT = reserva.DataCheckOUT;
            reservaGuardada.ValorPago = reserva.ValorPago;
            reservaGuardada.ValorPagoAux = reserva.ValorPagoAux;


            if (ModelState.IsValid)
            {
                try
                {

                    // Verifica se a data de check-in é inferior à data da reserva
                    if (reservaGuardada.DataCheckIN < reservaGuardada.DataReserva)
                    {
                        ModelState.AddModelError(nameof(reservaGuardada.DataCheckIN), "A data de check-in não pode ser anterior à data da reserva.");

                        // Recarregar as listas e devolve o controlo à View
                        ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome", reservaGuardada.QuartoFK);
                        var listaServicos = _context.Servicos.OrderBy(p => p.Nome).ToList();
                        ViewData["listaServicos"] = listaServicos;


                        return View(reservaGuardada);
                    }

                    // Verifica se o check-out é pelo menos um dia após o check-in
                    if (reservaGuardada.DataCheckIN >= reservaGuardada.DataCheckOUT)
                    {
                        ModelState.AddModelError(nameof(reservaGuardada.DataCheckOUT), "A data de check-out deve ser após a data de check-in.");

                        // Recarregar as listas e devolve o controlo à View
                        ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome", reservaGuardada.QuartoFK);
                        var listaServicos = _context.Servicos.OrderBy(p => p.Nome).ToList();
                        ViewData["listaServicos"] = listaServicos;
                        ViewData["servicosSelecionados"] = reservaGuardada.ListaServicos.Select(s => s.Id).ToList();

                        return View(reservaGuardada);
                    }

                    // Verifica se o check-out ocorre em um dia diferente do check-in
                    if (reservaGuardada.DataCheckIN.Date == reservaGuardada.DataCheckOUT.Date)
                    {
                        ModelState.AddModelError(nameof(reservaGuardada.DataCheckOUT), "A estadia deve incluir pelo menos uma noite.");

                        // Recarregar as listas e devolve o controlo à View
                        ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome", reservaGuardada.QuartoFK);
                        var listaServicos = _context.Servicos.OrderBy(p => p.Nome).ToList();
                        ViewData["listaServicos"] = listaServicos;
                        ViewData["servicosSelecionados"] = reservaGuardada.ListaServicos.Select(s => s.Id).ToList();

                        return View(reservaGuardada);
                    }


                    //transferir o valor de VAlorPagoAux para ValorPago
                    reservaGuardada.ValorPago = Convert.ToDecimal(reservaGuardada.ValorPagoAux.Replace('.', ','));

                    // Remover serviços antigos
                    reservaGuardada.ListaServicos.Clear();

                    // associar os serviços escolhidos à Reserva
                    // criar uma Lista de serviços
                    var listaServicosNaRes = new List<Servicos>();
                    foreach (var serv in escolhaServicos)
                    {
                        // procurar o Serviço na BD
                        var s = await _context.Servicos.FindAsync(serv);
                        if (s != null)
                        {
                            listaServicosNaRes.Add(s);
                        }
                    }
                    // atribuir a lista de serviços à Reserva
                    reservaGuardada.ListaServicos = listaServicosNaRes;

                    // Calcular o ValorTotal
                    reservaGuardada.ValorTotal = CalcularValorTotal(reservaGuardada);

                    // Calcula o valor que ainda falta pagar
                    reservaGuardada.ValorAPagar = reservaGuardada.ValorTotal - reservaGuardada.ValorPago;


                    // Verifica se o Valor Pago é superior ao Valor Total
                    if (reservaGuardada.ValorPago > reservaGuardada.ValorTotal)
                    {
                        ModelState.AddModelError("", "O Valor Pago não pode ser superior ao valor Total!");

                        // Recarregar as listas e devolve o controlo à View
                        ViewData["QuartoFK"] = new SelectList(_context.Quartos.OrderBy(q => q.Nome), "Id", "Nome", reservaGuardada.QuartoFK);
                        var listaServicos = _context.Servicos.OrderBy(p => p.Nome).ToList();
                        ViewData["listaServicos"] = listaServicos;
                        ViewData["servicosSelecionados"] = reservaGuardada.ListaServicos.Select(s => s.Id).ToList();

                        return View(reservaGuardada);
                    }

                    // Atualiza a tabela 
                    _context.Update(reservaGuardada);

                    // Efetua COMMIT na BD
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservasExists(reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuartoFK"] = new SelectList(_context.Quartos, "Id", "Id", reserva.QuartoFK);
            return View(reservaGuardada);
        }


        /* apenas as pessoas autenticadas E que pertençam 
         * ao Role de GERENTES ou Role de RECCECIONISTA podem entrar */
        [Authorize(Roles = "Gerentes, Reccecionistas")]
        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Quarto)
                .Include(r => r.ListaServicos)
                .Include(r => r.Hospede) // Incluir o Hospede na consulta
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            // Nas ações de DELETE também é crucial a existência
            // de Try Catch 

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservasExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }

        // Método auxiliar para calcular o valor total
        private decimal CalcularValorTotal(Reservas reserva)
        {
            decimal valorTotal = 0;

            // Calcula o número de noites no hotel
            TimeSpan duracao = reserva.DataCheckOUT.Date - reserva.DataCheckIN.Date;
            int numeroDeNoites = duracao.Days; // Usamos duracao.Days para garantir que o número de dias seja arredondado corretamente

            // Procura o objeto Quartos utilizando o Id (QuartoFK) da reserva
            var quarto = _context.Quartos.FirstOrDefault(q => q.Id == reserva.QuartoFK);

            // Calcula o valor do quarto por noite * nº de noites 
            valorTotal += numeroDeNoites * quarto.Preco;

            // Calcula o valor dos serviços
            if (reserva.ListaServicos != null)
            {
                foreach (var servico in reserva.ListaServicos)
                {
                    valorTotal += (servico.Preco * numeroDeNoites);
                }
            }

            return valorTotal;
        }
    }
}
