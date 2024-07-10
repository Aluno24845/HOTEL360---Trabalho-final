using HOTEL360___Trabalho_final.Data;
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

            var reservas = await _context.Reservas
                  .Include(r => r.Quarto)
                 .ToListAsync();

            return Ok(reservas);

        }


        // GET api/<ApiReservasController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {


            if (User.IsInRole("Gerentes") || User.IsInRole("Reccecionistas"))
            {
                var reserva = await _context.Reservas
                      .Include(r => r.Quarto)
                      .Include(r => r.ListaServicos)
                      .Include(r => r.Hospede)
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

            if (ModelState.IsValid && !haErros)
            {
                try
                {
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


            return BadRequest(new { erro = "A informacao enviada nao esta correta" });
        }

        // PUT api/<ApiReservasController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Reservas body)
        {

            var reserva = await _context.Reservas
                    .Include(r => r.Quarto)
                    .Include(r => r.ListaServicos)
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound(new { erro = "Reserva nao encontrada" });
            }

            //TODO verificar editar os servicos, tabela associativa

            /*reserva.ListaServicos = body.ListaServicos;*/
            reserva.ValorPagoAux = body.ValorPagoAux;
            reserva.ValorPago = body.ValorPago;
            reserva.DataCheckIN = body.DataCheckIN;
            reserva.DataCheckOUT = body.DataCheckOUT;
            reserva.QuartoFK = body.QuartoFK;

            _context.Update(reserva);
            await _context.SaveChangesAsync();

            return Ok(reserva);

        }

        // DELETE api/<ApiReservasController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return Ok(new { sucesso = true });

        }
        private bool ReservasExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
