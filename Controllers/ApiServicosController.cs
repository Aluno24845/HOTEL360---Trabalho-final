using HOTEL360___Trabalho_final.Data;
using HOTEL360___Trabalho_final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HOTEL360___Trabalho_final.Controllers
{
    [Route("api/servicos")]
    [ApiController]
    public class ApiServicosController : ControllerBase
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

        public ApiServicosController(
       ApplicationDbContext context,
       IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/<ApiServicosController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // retorna a lista de servicos
            return Ok(await _context.Servicos.ToListAsync());
        }

        // GET api/<ApiServicosController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            // verifica se o ID é nulo
            if (id == null)
            {
                return NotFound();
            }

            // procura o servico cujo ID é fornecido
            var reservas = await _context.Servicos
                .FirstOrDefaultAsync(m => m.Id == id);

            // caso o servico não for encontrado
            if (reservas == null)
            {
                return NotFound();
            }

            return Ok(reservas);
        }


        // POST api/<ApiServicosController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Servicos servico)
        {
            // verifica se os dados fornecidos pela View são válidos
            if (ModelState.IsValid)
            {

                try
                {

                    //transferir o valor de PrecoAux para Preco
                    servico.Preco = Convert.ToDecimal(servico.PrecoAux.Replace('.', ','));

                    //adiciona os dados vindos da View à BD
                    _context.Add(servico);
                    //efetua COMMIT na BD
                    await _context.SaveChangesAsync();
                    //redireciona o utilizador para a página Index
                    return Ok(servico);

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
            //se chegou aqui é porque algo não correu bem
            // mostra a mensagem de erro
            return BadRequest(new { error = "Informação incorreta" });
        }



        // PUT api/<ApiServicosController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Servicos servico)
        {
            //procura o servico cujo ID é fornecido
            var servicoGuardado = await _context.Servicos.FindAsync(id);

            // caso o servico não for encontrado
            if (servicoGuardado == null) { 
                return NotFound(new { erro = "Serviço não encontrado" }); 
            }

            // guarda os dados do servico
            servicoGuardado.Nome = servico.Nome;
            servicoGuardado.Descricao = servico.Descricao;
            servicoGuardado.PrecoAux = servico.PrecoAux;
            servicoGuardado.Preco = servico.Preco;

            // atualiza os dados do servico na BD
            _context.Update(servicoGuardado);
            // efetua COMMIT na BD
            await _context.SaveChangesAsync();

            return Ok(servicoGuardado);
        }

        // DELETE api/<ApiServicosController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // procura o servico cujo ID é fornecido
            var resersa = await _context.Servicos.FindAsync(id);

            // caso o servico não for encontrado
            if (resersa == null) { 
                return NotFound(new { erro = "Servico nao encontrado" }); 
            }

            // remove o servico da BD
            _context.Remove(resersa);
            // efetua COMMIT na BD
            await _context.SaveChangesAsync();
            return Ok(new { sucesso = true });
        }


    }
}