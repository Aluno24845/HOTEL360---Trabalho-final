﻿using HOTEL360___Trabalho_final.Data;
using HOTEL360___Trabalho_final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HOTEL360___Trabalho_final.Controllers
{    
    [Route("api/reservas")]
    [ApiController]
    public class ApiReservasController : ControllerBase
    {
        /// <summary>
        /// referência à BD do projeto
        /// </summary>
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// objeto que contém os dados referentes ao ambiente 
        /// do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ApiReservasController(
        ApplicationDbContext context,
        IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/<ApiReservasController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = new List<Reservas>();

            if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
            {
                 data = await _context.Reservas
                    .Include(r => r.Quarto)
                    .Include(r => r.Hospede) // Incluir o Hospede na consulta
                    .ToListAsync();
            }
            else
            {
                 data = await _context.Reservas
                    .Include(r => r.Quarto)
                    .Include(r => r.Hospede) // Incluir o Hospede na consulta
                    .Where(predicate: r => r.Hospede.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)) // apenas as reservas do utilizador autenticado
                    .ToListAsync();
            }

            // Remover as listas de Reservas e Hospedes para evitar referências cíclicas
            foreach (var r in data)
            {
                r.Quarto.ListaReservas = null;
                r.Hospede.ListaReservas = null;
            }

            return Ok(data);
        }


        // GET api/<ApiReservasController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {


            if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
            {
                var reserva = await _context.Reservas
                      .Include(r => r.Quarto)
                      .Include(r => r.Hospede)
                      .Include(r => r.ListaServicos)
                      .FirstOrDefaultAsync(m => m.Id == id);
                return Ok(reserva);
            }
            else
            {
                var reserva = await _context.Reservas
                     .Include(r => r.Quarto)
                     .Include(r => r.ListaServicos)
                     .FirstOrDefaultAsync(m => m.Id == id);
                return Ok(reserva);

            }
        }

        // POST api/<ApiReservasController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Reservas reserva)
        {

            // var. auxiliar
            bool haErros = false;

            if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
            {
                //Validações
                if (reserva.HospedeId == -1)
                {
                    haErros = true;
                    return BadRequest(new { erro = "Escolha um hospede, por favor." });
                }
            }
            else
            {
                // se o utilizador não for Gerente ou Rececionista
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(m => m.UserId == user);
                reserva.HospedeId = utilizador.Id;
            }

            //Validações
            if (reserva.QuartoFK == -1)
            {
                haErros = true;
                return BadRequest(new { erro = "Escolha um quarto, por favor." });
            }

            if (reserva.ListaServicos.Count() == 0)
            {
                // não escolhi nenhum serviço
                haErros = true;
                return BadRequest(new { erro = "Escolha um serviço, por favor." });
            }

            // Validações
            if (ModelState.IsValid && !haErros)
            {
                try
                {
                    // Guardar a lista de serviços na reserva
                    var listaServicosNaRes = new List<Servicos>();
                    foreach (var serv in reserva.ListaServicos)
                    {
                        var s = await _context.Servicos.FirstOrDefaultAsync(m => m.Id == serv.Id);
                        if (s != null)
                        {
                            listaServicosNaRes.Add(s);
                        }
                    }
                    reserva.ListaServicos = listaServicosNaRes;

                    //transferir o valor de VAlorPagoAux para ValorPago
                    reserva.ValorPago = Convert.ToDecimal(reserva.ValorPagoAux.Replace('.', ','));

                    var hospede = _context.Hospedes.Find(reserva.HospedeId);
                    //adiciona os dados vindos da View à BD
                    reserva.Hospede = hospede;
                    reserva.ValorTotal = CalcularValorTotal(reserva);
                    reserva.ValorAPagar = reserva.ValorTotal - reserva.ValorPago;

                    // guarda a reserva na BD
                    _context.Reservas.Add(reserva);

                    //efetua COMMIT na BD
                    await _context.SaveChangesAsync();

                    //redireciona o utilizador para a página Index
                    return Ok(reserva);
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
                    return BadRequest(new { erro = ex.Message });
                }

            }


            return BadRequest(new { erro = "A informação enviada não está correta." });
        }

        // PUT api/<ApiReservasController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Reservas body)
        {

            // Procura a reserva na BD
            var reserva = await _context.Reservas
                    .Include(r => r.Quarto) // Incluir o Quarto na consulta
                    .Include(r => r.ListaServicos) // Incluir a lista de Servicos na consulta
                    .FirstOrDefaultAsync(m => m.Id == id); // Procura a reserva pelo Id

            // Se a reserva não existir, devolve um erro
            if (reserva == null)
            {
                return NotFound(new { erro = "Reserva não encontrada" });
            }

            //TODO verificar editar os servicos, tabela associativa

            var listaServicosNaRes = new List<Servicos>();
            foreach (var serv in body.ListaServicos)
            {
                var s = await _context.Servicos.FirstOrDefaultAsync(m => m.Id == serv.Id);
                if (s != null)
                {
                    listaServicosNaRes.Add(s);
                }
            }

            // Atualiza os dados da reserva
            reserva.ListaServicos = listaServicosNaRes;            
            reserva.ValorPagoAux = body.ValorPagoAux;
            reserva.ValorPago = body.ValorPago;
            reserva.DataCheckIN = body.DataCheckIN;
            reserva.DataCheckOUT = body.DataCheckOUT;
            reserva.QuartoFK = body.QuartoFK;
            reserva.ValorTotal = CalcularValorTotal(reserva);
            reserva.ValorPago =   body.ValorPago;
            reserva.ValorAPagar = reserva.ValorTotal - reserva.ValorPago;

            //TODO lista de servicos - ja ha feito - tem que se iterar a lista de servicos e encontrar o que se quer

            // Guarda as alterações na BD
            _context.Reservas.Update(reserva);
            //efetua COMMIT na BD
            await _context.SaveChangesAsync();
                        
            return Ok(reserva);

        }

        // DELETE api/<ApiReservasController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Procura a reserva na BD
            var reserva = await _context.Reservas.FindAsync(id);

            // Se a reserva não existir, devolve um erro
            if (reserva == null)
            {
                return NotFound();
            }

            // Remove a reserva da BD
            _context.Reservas.Remove(reserva);

            //efetua COMMIT na BD
            await _context.SaveChangesAsync();

            return Ok(new { sucesso = true });

        }


        // Função que verifica se uma reserva existe
        private bool ReservasExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }


        // Função que calcula o valor total de uma reserva
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

            // Retorna o valor total
            return valorTotal;
        }
    }
}
