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
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Hosting;
using System.Dynamic;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HOTEL360___Trabalho_final.Controllers
{
    [Route("api/utilizadores")]
    [ApiController]
    [Authorize]
    public class ApiUtilizadoresController : ControllerBase
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
        /// <summary>
        /// referência para gerar Hash para password
        /// </summary>
        public readonly IPasswordHasher<IdentityUser> _passwordHasher;

        public readonly UserManager<IdentityUser> _userManager;


        public ApiUtilizadoresController(
            ApplicationDbContext context, 
            IPasswordHasher<IdentityUser> passwordHasher,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }



        // GET: api/<ApiUtilizadoresController>
        [Authorize(Roles = "Gerentes")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // retorna a lista de utilizadores
            var hospedes = await _context.Hospedes.ToListAsync();
            var gerentes = await _context.Gerentes.ToListAsync();
            var rececionistas = await _context.Reccecionistas.ToListAsync();
            return Ok(new { hospedes, gerentes, rececionistas });
        }

        //GET hospedes
        [Authorize(Roles = "Gerentes")]
        [HttpGet("hospedes", Name = "hospedes")]
        public async Task<IActionResult> GetHospedes()
        {
            // retorna a lista de hospedes
            return Ok(await _context.Hospedes.ToListAsync());
        }


        [Authorize]
        [HttpGet("myself", Name = "myself")]
        public async Task<IActionResult> GetMyself()
        {
            // verifica se o utilizador está autenticado
            if (!User.Identity.IsAuthenticated)
            {
                // se cheguei aqui é pq o utilizador não está autenticado
                return Unauthorized(new { erro = "Por favor faça login" });
            }
            // obter o ID do utilizador autenticado
            var myself = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // caso o ID seja nulo
            if (User == null)
            {
                return Unauthorized(new { erro = "Sessão Expirou" });
            }

            // obter a role do utilizador autenticado
            var role = User.FindFirst(ClaimTypes.Role).Value;
            // obter o utilizador autenticado
            var user = await _context.Users.FindAsync(myself);

            // caso o utilizador seja do role HOSPEDES
            if (role == "Hospedes")
            {
                // obter o utilizador autenticado
                var utilizador = await _context.Hospedes.FirstOrDefaultAsync(m => m.UserId == user.Id);
                return Ok(new { utilizador, role });
            }
            // caso o utilizador seja do role RECCECIONISTAS
            else if (role == "Reccecionistas")
            {
                // obter o utilizador autenticado
                var utilizador = await _context.Reccecionistas.FirstOrDefaultAsync(m => m.UserId == user.Id);
                return Ok(new { utilizador, role });

            }
            // caso o utilizador seja do role GERENTES
            else if (role == "Gerentes")
            {
                // obter o utilizador autenticado
                var utilizador = await _context.Gerentes.FirstOrDefaultAsync(m => m.UserId == user.Id);
                return Ok(new { utilizador, role });
            }
            else
            {
                // caso o utilizador não tenha role associada
                return Unauthorized(new { erro = "Não tem role associada" });
            }
        }


        // GET api/<ApiUtilizadoresController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            // obter o ID do utilizador autenticado
            var myself = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // obter a role do utilizador autenticado
            var role = User.FindFirst(ClaimTypes.Role).Value;
            // obter o utilizador autenticado
            var user = await _context.Users.FindAsync(myself);

            // procura o utilizador cujo ID é fornecido na tabela Utilizadores
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);

            // verificar se o utilizador autenticado é do role HOSPEDES
            if (role == "Hospedes" && utilizador.UserId != user.Id)
            {
                return Unauthorized(new { erro = "Não tem permissão para aceder a este utilizador" });
            }

            // verifica se o utilizador é nulo
            if (utilizador == null)
            {
                return NotFound(new { erro = "Utilizador id não encontrado!" });
            }

            // obbter o utilizador da tabela AspNetUsers
            var aspUser = await _context.Users.FindAsync(utilizador.UserId);

            // verificar se o utilizador é nulo
            if (aspUser == null)
            {
                return NotFound(new { erro = "Utilizador id não encontrado!" });
            }

            // criar um objeto dinâmico
            dynamic data = new ExpandoObject();

            // adicionar os dados ao objeto dinâmico
            data.role = role;
            data.email = aspUser.Email;
            data.id = utilizador.Id;
            data.nome = utilizador.Nome;
            data.telemovel = utilizador.Telemovel;
            data.avatar = utilizador.Avatar;
            data.dataNascimento = utilizador.DataNascimento;

            // procura a role do utilizador
            var roleUtilizador =await _userManager.GetRolesAsync(aspUser);

            // verificar se o utilizador é do role GERENTES
            if (roleUtilizador.First() == "Gerentes")
            {
                // obter o gerente
                var gerente = await _context.Gerentes.FirstOrDefaultAsync(m => m.UserId == aspUser.Id);
                data.nif = gerente.NIF;
            }
            // verificar se o utilizador é do role RECCECIONISTAS
            else if (roleUtilizador.First() == "Reccecionistas")
            {
                // obter o reccecionista
                var reccecionista = await _context.Reccecionistas.FirstOrDefaultAsync(m => m.UserId == aspUser.Id);

            }
            // verificar se o utilizador é do role HOSPEDES
            else if (roleUtilizador.First() == "Hospedes")
            {
                // obter o hospede
                var hospede = await _context.Hospedes.FirstOrDefaultAsync(m => m.UserId == aspUser.Id);
                data.nif = hospede.NIF;
            }
            else
            {
                return NotFound(new { erro = "Utilizador não encontrado" });
            }
            // retornar os dados
            return Ok(data);
        }


        // POST api/<ApiUtilizadoresController>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CriarUtilizadores criarutilizador, [FromForm] IFormFile ImagemLogo)
        {
            // verificar se o utilizador está autenticado e se é OU do role Gerentes OU do role Reccecionistas
            if (!User.Identity.IsAuthenticated && (criarutilizador.Tipo == "Gerentes" || criarutilizador.Tipo == "Reccecionistas"))
            {
                return Unauthorized(new { erro = "Requer autenticação" });
            }

            // vars. auxiliares
            string nomeImagem = "";
            bool haImagem = false;

            // verificar se existe ficheiro
            if (ImagemLogo != null)
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
                }
            }

            // verificar se os dados fornecidos são válidos
            if (ModelState.IsValid)
            {
                // criar um novo utilizador
                IdentityUser applicationUser = new IdentityUser();
                Guid guid = Guid.NewGuid();
                applicationUser.Id = guid.ToString();
                applicationUser.UserName = criarutilizador.Email;
                applicationUser.NormalizedUserName = criarutilizador.Email.ToUpper();
                applicationUser.Email = criarutilizador.Email;
                applicationUser.NormalizedEmail = criarutilizador.Email.ToUpper();
                applicationUser.EmailConfirmed = true;
                var hashedPassword = _passwordHasher.HashPassword(applicationUser, criarutilizador.Password);
                applicationUser.SecurityStamp = Guid.NewGuid().ToString();
                applicationUser.PasswordHash = hashedPassword;

                // adicionar o utilizador à BD
                _context.Users.Add(applicationUser);
                // efetuar COMMIT na BD
                await _context.SaveChangesAsync();

                // obter o utilizador da BD
                var usercreated = await _context.Users.FindAsync(applicationUser.Id);

                // verificar se o utilizador criado é do role GERENTES
                if (criarutilizador.Tipo == "Gerentes")
                {
                    // criar um novo gerente
                    Gerentes utilizador = new Gerentes();
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NIF = criarutilizador.NIF;
                    utilizador.UserId = applicationUser.Id;

                    // verificar se existe imagem
                    if (haImagem)
                    {
                        utilizador.Avatar = nomeImagem;
                    }

                    // adicionar o gerente à BD
                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role GERENTES
                    await _userManager.AddToRoleAsync(applicationUser, "Gerentes");
                    // **********************************************

                }
                // verificar se o utilizador criado é do role HOSPEDES
                else if (criarutilizador.Tipo == "Hospedes")
                {
                    // criar um novo hospede
                    Hospedes utilizador = new Hospedes();
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NIF = criarutilizador.NIF;
                    utilizador.UserId = applicationUser.Id;

                    // verificar se existe imagem
                    if (haImagem)
                    {
                        utilizador.Avatar = nomeImagem;
                    }

                    // adicionar o hospede à BD
                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role HOSPEDES
                    await _userManager.AddToRoleAsync(applicationUser, "Hospedes");
                    // **********************************************


                }
                // verifica se o utilizador criado é do role RECCECIONISTAS
                else if (criarutilizador.Tipo == "Reccecionistas")
                {
                    // criar um novo reccecionista
                    Reccecionistas utilizador = new Reccecionistas();
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.UserId = applicationUser.Id;

                    // verificar se existe imagem
                    if (haImagem)
                    {
                        utilizador.Avatar = nomeImagem;
                    }

                    // adicionar o reccecionista à BD
                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role RECCECIONISTAS
                    await _userManager.AddToRoleAsync(applicationUser, "Reccecionistas");
                    // **********************************************

                }
                // caso não tenha tipo então significa que é um registo (registo só pode ser hospede)
                else
                {
                    // criar um novo hospede
                    Hospedes utilizador = new Hospedes();
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NIF = criarutilizador.NIF;
                    utilizador.UserId = applicationUser.Id;

                    // verificar se existe imagem
                    if (haImagem)
                    {
                        utilizador.Avatar = nomeImagem;
                    }

                    // adicionar o hospede à BD
                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role HOSPEDES
                    await _userManager.AddToRoleAsync(applicationUser, "Hospedes");
                    // **********************************************

                }

                try
                {

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

                    // efetuar COMMIT na BD
                    await _context.SaveChangesAsync();

                    //redireciona o utilizador para a página Index 
                    return Ok(new { sucesso = true });
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
            else
            {
                return BadRequest(new { erro = "Error a criar conta de utilizador" });
            }

        }



        // PUT api/<ApiUtilizadoresController>/5
        //EDITAR CONTA
        //trocar password, telemovel
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] EditarUtilizador editarUtilizador, [FromForm] IFormFile ImagemLogo)
        {
            // procua o utilizador cujo ID é fornecido
            var utilizador = await _context.Utilizadores
            .FirstOrDefaultAsync(m => m.Id == id);

            // verificar se o utilizador é nulo
            if (utilizador == null)
            {
                return NotFound(new { erro = "Utilizador não encontrado" });
            }

            /* if (editarUtilizador.Password!=null)
             {
                 var identityUser = await _context.Users.FindAsync(utilizador.UserId);

                 var hashedPassword = _passwordHasher.HashPassword(identityUser, editarUtilizador.Password);
                 identityUser.SecurityStamp = Guid.NewGuid().ToString();
                 identityUser.PasswordHash = hashedPassword;
                 _context.Users.Update(identityUser);
                 await _context.SaveChangesAsync();
             }*/

            // verificar se existe imagem
            string nomeImagem = "";
            bool haImagem = false;

            // verificar se existe ficheiro
            if (ImagemLogo != null)
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
                }
            }

            try
            {

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

                // obter o ID do utilizador autenticado
                var role = User.FindFirst(ClaimTypes.Role).Value;

                // verificar se o utilizador é do role GERENTES
                if (role.Contains("Gerentes"))
                {
                    // obter o gerente
                    var gerente = await _context.Gerentes.FirstOrDefaultAsync(m => m.Id == id);

                    // verificar se o NIF é diferente
                    if (editarUtilizador.NIF != null && editarUtilizador.NIF != gerente.NIF)
                    {
                        // atualizar o NIF
                        gerente.NIF = editarUtilizador.NIF;
                    }
                    // verificar se o telemovel é diferente
                    if (editarUtilizador.Telemovel != null && editarUtilizador.Telemovel != gerente.Telemovel)
                    {
                        // atualizar o telemovel
                        gerente.Telemovel = editarUtilizador.Telemovel;
                    }
                    // verificar se existe imagem
                    if (haImagem)
                    {
                        // atualizar a imagem
                        gerente.Avatar = nomeImagem;
                    }
                    // atualizar o gerente
                    _context.Update(gerente);
                }
                // verificar se o utilizador é o role RECCECIONISTAS
                else if (role.Contains("Reccecionistas"))
                {
                    // obter o reccecionista
                    var reccecionista = await _context.Reccecionistas.FirstOrDefaultAsync(m => m.Id == id);
                    // verifica se existe imagem
                    if (haImagem)
                    {
                        // atualizar a imagem
                        reccecionista.Avatar = nomeImagem;
                    }
                    // atualizar o reccecionista
                    _context.Update(reccecionista);

                }
                // verificar se o utilizador é do role HOSPEDES
                else if (role.Contains("Hospedes"))
                {
                    // obter o hospede
                    var hospede = await _context.Hospedes.FirstOrDefaultAsync(m => m.Id == id);

                    // verificar se o NIF é diferente
                    if (editarUtilizador.NIF != null && editarUtilizador.NIF != hospede.NIF)
                    {
                        // atualizar o NIF
                        hospede.NIF = editarUtilizador.NIF;
                    }
                    // verificar se o telemovel é diferente
                    if (editarUtilizador.Telemovel != null && editarUtilizador.Telemovel != hospede.Telemovel)
                    {
                        // atualizar o telemovel
                        hospede.Telemovel = editarUtilizador.Telemovel;
                    }
                    // verificar se existe imagem
                    if (haImagem)
                    {
                        // atualizar a imagem
                        hospede.Avatar = nomeImagem;
                    }
                    // atualizar o hospede
                    _context.Update(hospede);
                }

                //TODO adicionar validacoes de user/roles
                // verificar se o email é nulo
                if (editarUtilizador.Email != null)
                {
                    // obter o utilizador
                    var identityUser = await _context.Users.FindAsync(utilizador.UserId);
                    // atualizar o email
                    identityUser.Email = editarUtilizador.Email;
                    identityUser.UserName = editarUtilizador.Email;
                    identityUser.NormalizedUserName = editarUtilizador.Email.ToUpper();
                    identityUser.NormalizedEmail = editarUtilizador.Email.ToUpper();

                    // atualizar o utilizador
                    _context.Users.Update(identityUser);
                }
                // evetuar COMMIT na BD
                await _context.SaveChangesAsync();
                                
                return Ok(new { sucesso = true });
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
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }

        }



        // DELETE api/<ApiUtilizadoresController>/5
        [Authorize(Roles = "Gerentes")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // procura o utilizador cujo ID é fornecido
            var utilizador = await _context.Utilizadores
                    .FirstOrDefaultAsync(m => m.Id == id);

            // caso o utilizador não for encontrado
            if (utilizador == null)
            {
                return NotFound(new { err = "Utilizador não encontrado" });
            }

            // remove o utilizador da BD
            _context.Utilizadores.Remove(utilizador);

            // procura o utilizador na tabela AspNetUsers
            var aspUser = await _context.Users.FindAsync(utilizador.UserId);

            // remove o utilizador da tabela AspNetUsers
            _context.Users.Remove(aspUser);

            // efetua COMMIT na BD
            await _context.SaveChangesAsync();

            return Ok(new { sucesso = true });
        }
    }
}
