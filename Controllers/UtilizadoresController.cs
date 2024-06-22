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

            var utilizadores = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizadores == null)
            {
                return NotFound();
            }

            return View(utilizadores);
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
                if (criarutilizador.Tipo == "Gerentes")  {
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
                  
                } else if (criarutilizador.Tipo == "Hospedes")
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
                    

                } else if (criarutilizador.Tipo == "Reccecionistas")
                {
                    Reccecionistas utilizador = new Reccecionistas();
                    utilizador.Nome = criarutilizador.Nome;
                    utilizador.Telemovel = criarutilizador.Telemovel;
                    utilizador.DataNascimento = criarutilizador.DataNascimento;
                    utilizador.Avatar = criarutilizador.Avatar;
                    utilizador.NumReccecionista = criarutilizador.NumReccecionista;
                    utilizador.UserId = applicationUser.Id;

                    _context.Add(utilizador);

                    // **********************************************
                    // Vamos atribuir à pessoa que registámos o Role RECCECIONISTAS
                    await _userManager.AddToRoleAsync(applicationUser, "Reccecionistas");
                    // **********************************************

                }

                await _context.SaveChangesAsync();

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

            var utilizadores = await _context.Utilizadores.FindAsync(id);
            if (utilizadores == null)
            {
                return NotFound();
            }
            return View(utilizadores);
        }

        // POST: Utilizadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Telemovel,Avatar,DataNascimento,UserId")] Utilizadores utilizadores)
        {
            if (id != utilizadores.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilizadores);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadoresExists(utilizadores.Id))
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
            return View(utilizadores);
        }

        // GET: Utilizadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadores = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizadores == null)
            {
                return NotFound();
            }

            return View(utilizadores);
        }

        // POST: Utilizadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizadores = await _context.Utilizadores.FindAsync(id);
            if (utilizadores != null)
            {
                _context.Utilizadores.Remove(utilizadores);
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
