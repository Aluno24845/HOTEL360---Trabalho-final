using HOTEL360___Trabalho_final.Data;
using HOTEL360___Trabalho_final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HOTEL360___Trabalho_final.Controllers
{
    [Route("api/quartos")]
    [ApiController]
    public class ApiQuartosController : ControllerBase
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

        public ApiQuartosController(
       ApplicationDbContext context,
       IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/<ApiQuartosController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // retorna a lista de quartos
            return Ok(await _context.Quartos.ToListAsync());
        }


        // GET api/<ApiQuartosController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            // procura o quarto cujo ID é fornecido
            var quarto = await _context.Quartos
                .FirstOrDefaultAsync(m => m.Id == id);

            // caso o quarto não for encontrado
            if (quarto == null)
            {
                return NotFound(new { erro = "Quarto não encontrado" });
            }

            return Ok(quarto);
        }

        // POST api/<ApiQuartosController>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Quartos quarto, [FromForm] IFormFile ImagemLogo)
        {


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
                return BadRequest(new { erro = "O fornecimento de uma Imagem é obrigatório!" });
            }
            else
            {
                // há ficheiro, mas é imagem?
                if (!(ImagemLogo.ContentType == "image/png" || ImagemLogo.ContentType == "image/jpeg"))
                {
                    return BadRequest(new { erro = "Tem de fornecer para a Imagem um ficheiro PNG ou JPG!" });
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
            if (ModelState.IsValid)
            {

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
                    return Ok(quarto);
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
            return Ok(quarto);
        }


        // PUT api/<ApiQuartosController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Quartos quarto, [FromForm] IFormFile ImagemLogo)
        {
           // procura o quarto cujo ID é fornecido
            var quartoGuardado = await _context.Quartos.FindAsync(id);

            // caso o quarto não for encontrado
            if (quartoGuardado == null)
            {
                return NotFound(new { erro = "Quarto não encontrado" });
            }

            string nomeImagem = "";
            bool haImagem = false;


            // há ficheiro, mas é imagem?
            if (ImagemLogo != null)
            {

                if (!(ImagemLogo.ContentType == "image/png" || ImagemLogo.ContentType == "image/jpeg"))
                {
                    return BadRequest(new { erro = "Tem de fornecer para a Imagem um ficheiro PNG ou JPG!" });
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
                    quartoGuardado.Imagem = nomeImagem;
                }
            }
            try
            {
                // guarda os dados do quarto
                quartoGuardado.Localizacao = quarto.Localizacao;
                quartoGuardado.Nome = quarto.Nome;
                quartoGuardado.Descricao = quarto.Descricao;
                quartoGuardado.PrecoAux = quarto.PrecoAux;
                quartoGuardado.Preco = Convert.ToDecimal(quarto.PrecoAux.Replace('.', ','));
                quartoGuardado.Capacidade = quarto.Capacidade;

                // atualiza os dados do quarto na BD
                _context.Update(quartoGuardado);

                //efetua COMMIT na BD
                await _context.SaveChangesAsync();

                // se há ficheiro de imagem,
                // vamos guardar no disco rígido do servidor
                if (haImagem)
                {
                    // determinar onde se vai guardar a imagem
                    string nomePastaOndeGuardarImagem = _webHostEnvironment.WebRootPath;
                    // já sei o caminho até à pasta wwwroot
                    // especifico onde vou guardar a imagem
                    nomePastaOndeGuardarImagem = Path.Combine(nomePastaOndeGuardarImagem, "Imagens");
                    // e, existe a pasta 'Imagens'?
                    if (!Directory.Exists(nomePastaOndeGuardarImagem))
                    {
                        Directory.CreateDirectory(nomePastaOndeGuardarImagem);
                    }
                    // juntar o nome do ficheiro à sua localização
                    string nomeFinalDaImagem = Path.Combine(nomePastaOndeGuardarImagem, nomeImagem);

                    // guardar a imagem no disco rigído
                    using var stream = new FileStream(nomeFinalDaImagem, FileMode.Create);
                    await ImagemLogo.CopyToAsync(stream);
                }

                //redireciona o utilizador para a página Index 
                return Ok(quartoGuardado);
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


        // DELETE api/<ApiQuartosController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // procura o quarto cujo ID é fornecido
            var quarto = await _context.Quartos.FindAsync(id);

            // caso o quarto não for encontrado
            if (quarto == null)
            {
                return NotFound(new { erro = "Quarto não encontrado" });
            }

            // remove o quarto da BD
            var quartos = _context.Quartos.Remove(quarto);
            //efetua COMMIT na BD
            await _context.SaveChangesAsync();

            return Ok(new { successo = true });

        }
    }
}
