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
        /// referência para gerar Hash para password
        /// </summary>
        public readonly IPasswordHasher<IdentityUser> _passwordHasher;

        public readonly UserManager<IdentityUser> _userManager;


        public ApiUtilizadoresController(ApplicationDbContext context, IPasswordHasher<IdentityUser> passwordHasher, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
        }



        // GET: api/<ApiUtilizadoresController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
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
            return Ok(await _context.Hospedes.ToListAsync());
        }

        [Authorize]
        [HttpGet("myself", Name = "myself")]
        public async Task<IActionResult> GetMyself()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { erro = "Por favor faca login" });
            }
            var myself = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (User == null)
            {
                return Unauthorized(new { erro = "Sessao Expirou" });
            }

            var role = User.FindFirst(ClaimTypes.Role).Value;
            var user = await _context.Users.FindAsync(myself);

            if (role == "Hospedes")
            {

                var utilizador = await _context.Hospedes.FirstOrDefaultAsync(m => m.UserId == user.Id);
                return Ok(new { utilizador, role });
            }
            else if (role == "Reccecionistas")
            {
                var utilizador = await _context.Reccecionistas.FirstOrDefaultAsync(m => m.UserId == user.Id);
                return Ok(new { utilizador, role });

            }
            else if (role == "Gerentes")
            {
                var utilizador = await _context.Gerentes.FirstOrDefaultAsync(m => m.UserId == user.Id);
                return Ok(new { utilizador, role });
            }
            else
            {
                return Unauthorized(new { erro = "Nao tem role associada" });
            }
        }
        // GET api/<ApiUtilizadoresController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound(new { erro = "Utilizador id nao encontrado!" });
            }

            return Ok(utilizador);
        }

        // POST api/<ApiUtilizadoresController>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriarUtilizadores criarutilizador)
        {

            if (!User.Identity.IsAuthenticated && (criarutilizador.Tipo == "Gerentes" || criarutilizador.Tipo == "Reccecionistas"))
            {
                return Unauthorized(new { erro = "Requer autenticacao" });
            }

            if (ModelState.IsValid)
            {
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

                _context.Users.Add(applicationUser);
                await _context.SaveChangesAsync();

                var usercreated = await _context.Users.FindAsync(applicationUser.Id);
                if (criarutilizador.Tipo == "Gerentes")
                {
                    Gerentes utilizador = new Gerentes();
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NIF = criarutilizador.NIF;
                    utilizador.UserId = applicationUser.Id;

                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role GERENTES
                    await _userManager.AddToRoleAsync(applicationUser, "Gerentes");
                    // **********************************************

                }
                else if (criarutilizador.Tipo == "Hospedes")
                {
                    Hospedes utilizador = new Hospedes();
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NIF = criarutilizador.NIF;
                    utilizador.UserId = applicationUser.Id;

                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role HOSPEDES
                    await _userManager.AddToRoleAsync(applicationUser, "Hospedes");
                    // **********************************************


                }
                else if (criarutilizador.Tipo == "Reccecionistas")
                {
                    Reccecionistas utilizador = new Reccecionistas();
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.UserId = applicationUser.Id;

                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role RECCECIONISTAS
                    await _userManager.AddToRoleAsync(applicationUser, "Reccecionistas");
                    // **********************************************

                }
                else
                {
                    Hospedes utilizador = new Hospedes();
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NIF = criarutilizador.NIF;
                    utilizador.UserId = applicationUser.Id;

                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role HOSPEDES
                    await _userManager.AddToRoleAsync(applicationUser, "Hospedes");
                    // **********************************************

                }

                await _context.SaveChangesAsync();
                return Ok(new { sucesso = true });


            }
            return BadRequest(new { erro = "Error a criar conta de utilizador" });
        }

        // PUT api/<ApiUtilizadoresController>/5
        //EDITAR CONTA
        //trocar password, telemovel
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CriarUtilizadores criarutilizador)
        {

            var utilizador = await _context.Utilizadores
            .FirstOrDefaultAsync(m => m.Id == id);

            if (utilizador == null)
            {
                return NotFound(new { err = "Utilizador nao encontrado" });
            }

            if (criarutilizador.Password != null)
            {
                var identityUser = await _context.Users.FindAsync(utilizador.UserId);

                var hashedPassword = _passwordHasher.HashPassword(identityUser, criarutilizador.Password);
                identityUser.SecurityStamp = Guid.NewGuid().ToString();
                identityUser.PasswordHash = hashedPassword;
                _context.Users.Update(identityUser);
                await _context.SaveChangesAsync();
            }
            if (criarutilizador.Telemovel != null && criarutilizador.Telemovel != utilizador.Telemovel)
            {
                utilizador.Telemovel = criarutilizador.Telemovel;
            }
            _context.Update(utilizador);
            await _context.SaveChangesAsync();
            return Ok(utilizador);

        }

        // DELETE api/<ApiUtilizadoresController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
