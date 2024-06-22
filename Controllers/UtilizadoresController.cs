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
         * ao Role de GERENTES podem entrar */
    [Authorize(Roles = "Gerentes")]
    public class UtilizadoresController : Controller  {

        /// <summary>
        /// referência à BD do projeto
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// referência para gerar Hash para password
        /// </summary>
        public readonly IPasswordHasher<IdentityUser> _passwordHasher;

        /// <summary>
        ///  Objeto para interagir com a Autenticação
        /// </summary>
        public readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Objeto para interagir com as Roles
        /// </summary>
        private readonly RoleManager<IdentityRole> _roleManager;

        public UtilizadoresController(ApplicationDbContext context, IPasswordHasher<IdentityUser> passwordHasher, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Utilizadores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizadores.ToListAsync());
        }

        // GET: Utilizadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // GET: Utilizadores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilizadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email, Password, ConfirmPassword,Nome,Telemovel,Avatar,DataNascimento,NIF, Tipo, NumReccecionista")] CriarUtilizadores criarutilizador)
        {
           
            //verifica se o modelo é válido
            if (ModelState.IsValid) {
                // Cria uma nova instância de IdentityUser
                IdentityUser applicationUser = new IdentityUser();
                // Gera um novo GUID para o Id do utilizador
                Guid guid = Guid.NewGuid();
                applicationUser.Id = guid.ToString();
                // Define o nome de utilizador como o email fornecido
                applicationUser.UserName = criarutilizador.Email;
                // Normaliza o nome de utilizador (converte para maiúsculas)
                applicationUser.NormalizedUserName = criarutilizador.Email.ToUpper();
                // Define o email do utilizador
                applicationUser.Email = criarutilizador.Email;
                // Normaliza o email 
                applicationUser.NormalizedEmail = criarutilizador.Email.ToUpper();
                //Define que o email já foi confirmado
                applicationUser.EmailConfirmed = true;
                // Gera o hash da password do utilizador
                var hashedPassword = _passwordHasher.HashPassword(applicationUser, criarutilizador.Password);
                // Gera um SecurityStamp 
                applicationUser.SecurityStamp = Guid.NewGuid().ToString();
                // Define o hash da password gerado
                applicationUser.PasswordHash = hashedPassword;

                // Adiciona o utilizador à tabela de Users
                _context.Users.Add(applicationUser);
                // Guarda as mudanças na bd
                await _context.SaveChangesAsync();

                // Procuramos o utilizador recém-criado pelo Id
                var usercreated = await _context.Users.FindAsync(applicationUser.Id);
                if (criarutilizador.Tipo == "Gerentes")  {
                    // Se o tipo de utiizador for "Gerentes", criamos um novo objeto do tipo Gerentes
                    Gerentes utilizador = new Gerentes();
                    // Atribuimos as propriedades específicas do Gerente
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NIF = criarutilizador.NIF;
                    utilizador.UserId = applicationUser.Id;

                    // Adicionamos o objeto Gerentes à tabela Utilizadores
                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role GERENTES
                    await _userManager.AddToRoleAsync(applicationUser, "Gerentes");
                    // **********************************************
                  
                } else if (criarutilizador.Tipo == "Hospedes")  {
                    // Se o tipo de utilizador for "Hospedes", criamos um novo objeto do tipo Hospedes
                    Hospedes utilizador = new Hospedes();
                    // Atribuimos as propriedades específicas do Hospede
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NIF = criarutilizador.NIF;
                    utilizador.UserId = applicationUser.Id;

                    // Adicionamos o objeto Hospedes à tabela Utilizadores
                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role HOSPEDES
                    await _userManager.AddToRoleAsync(applicationUser, "Hospedes");
                    // **********************************************
                    

                } else if (criarutilizador.Tipo == "Reccecionistas")  {
                    // Se o tipo de utilizador for "Reccecionistas", criamos um novo objeto do tipo Reccecionistas
                    Reccecionistas utilizador = new Reccecionistas();
                    // Atribuimos as propriedades específicas do Reccecionista
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NumReccecionista = criarutilizador.NumReccecionista;
                    utilizador.UserId = applicationUser.Id;

                    // Adicionamos o objeto Reccecionistas à tabela Utilizadores
                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role RECCECIONISTAS
                    await _userManager.AddToRoleAsync(applicationUser, "Reccecionistas");
                    // **********************************************

                }

                // Guardamos as alterações na base de dados
                await _context.SaveChangesAsync();

                // Somos rederecionados para a página Index 
                return RedirectToAction(nameof(Index));


            }
            return View(criarutilizador);
        }

        // GET: Utilizadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound();
            }
            return View(utilizador);
        }

        // POST: Utilizadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Telemovel,Avatar,DataNascimento,UserId")] Utilizadores utilizador)
        {
            if (id != utilizador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilizador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadoresExists(utilizador.Id))
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
            return View(utilizador);
        }

        // GET: Utilizadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // POST: Utilizadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador != null)
            {
                _context.Utilizadores.Remove(utilizador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizadoresExists(int id)
        {
            return _context.Utilizadores.Any(e => e.Id == id);
        }
    }
}
