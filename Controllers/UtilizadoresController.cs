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
        /// objeto que contém os dados referentes ao ambiente 
        /// do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;

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

        public UtilizadoresController(
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            IPasswordHasher<IdentityUser> passwordHasher, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager)  {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

            // Carregar o utilizador incluindo os dados do ASP.NET Identity (AspNetUsers)
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            // Buscar o email do utilizador a partir do ASP.NET Identity (AspNetUsers)
            var userEmail = await _context.Users
                .Where(u => u.Id == utilizador.UserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            if (userEmail == null)
            {
                return NotFound();
            }

            ViewData["UserEmail"] = userEmail;

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
        public async Task<IActionResult> Create([Bind("Email, Password, ConfirmPassword,Nome,Telemovel,Avatar,DataNascimento,NIF, Tipo, NumReccecionista")] CriarUtilizadores criarutilizador, IFormFile ImagemLogo)
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
        *                  e pedir PNG ou JPG.......
        *          1.2.2 - é uma imagem
        *                  - determinar o nome a atribuir 
        *                    ao ficheiro
        *                  - escrever esse nome na BD
        *                  - se a escrita na BD se concretizar
        *                    é que o ficheiro é guardado no 
        *                    disco rígido
        */
            //Validação do tipo
            if (criarutilizador.Tipo == "-1")
            {
                ModelState.AddModelError("", "Escolha um tipo de utilizador, por favor.");
                return View(criarutilizador);
            }

            // vars. auxiliares
            string nomeImagem = "";
            bool haImagem = false;

            // há ficheiro?
            if (ImagemLogo == null)
            {
                ModelState.AddModelError("",
                   "O fornecimento de um Avatar é obrigatório!");
                return View(criarutilizador);
            }
            else
            {
                // há ficheiro, mas é imagem?
                if (!(ImagemLogo.ContentType == "image/png" ||
                       ImagemLogo.ContentType == "image/jpeg")
                   )
                {
                    ModelState.AddModelError("",
                   "Tem de fornecer para o Avatar um ficheiro PNG ou JPG!");
                    return View(criarutilizador);
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
                    criarutilizador.Avatar = nomeImagem;
                }
            }

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

            // Carrega o utilizador da tabela AspNetUsers pelo UserId do utilizador
            var user = await _userManager.FindByIdAsync(utilizador.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Cria um modelo para a edição com os dados do utilizador e UserId
            var model = new Utilizadores
            {
                Id = utilizador.Id,
                Nome = utilizador.Nome,
                Telemovel = utilizador.Telemovel,
                Avatar = utilizador.Avatar,
                DataNascimento = utilizador.DataNascimento,
                UserId = user.Id // Passa o UserId para o modelo
            };

            return View(model);

        }

        // POST: Utilizadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Telemovel,Avatar,DataNascimento,UserId")] Utilizadores utilizador, IFormFile ImagemLogo)
        {
            if (id != utilizador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImagemLogo != null && (ImagemLogo.ContentType == "image/png" || ImagemLogo.ContentType == "image/jpeg"))
                    {
                        // Obter o nome a atribuir à imagem
                        Guid g = Guid.NewGuid();
                        string nomeImagem = g.ToString();
                        // Obter a extensão do nome da imagem
                        string extensao = Path.GetExtension(ImagemLogo.FileName);
                        nomeImagem += extensao;

                        // Define o caminho para guardar a imagem
                        string nomePastaOndeGuardarImagem = Path.Combine(_webHostEnvironment.WebRootPath, "Imagens");
                        if (!Directory.Exists(nomePastaOndeGuardarImagem))
                        {
                            Directory.CreateDirectory(nomePastaOndeGuardarImagem);
                        }
                        string nomeFinalDaImagem = Path.Combine(nomePastaOndeGuardarImagem, nomeImagem);

                        // Guarda a imagem
                        using var stream = new FileStream(nomeFinalDaImagem, FileMode.Create);
                        await ImagemLogo.CopyToAsync(stream);

                        // Atualiza o avatar do utilizador
                        utilizador.Avatar = nomeImagem;
                    }
                    else
                    {
                        // Não atualize o campo Avatar se nenhuma nova imagem for carregada
                        _context.Entry(utilizador).Property(u => u.Avatar).IsModified = false;
                    }
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

            // Encontra o utilizador na tabela AspNetUsers pelo Id do Utilizador
            var user = await _userManager.FindByIdAsync(utilizador.UserId);
            if (user != null)
            {
                // Remove o utilizador da tabela AspNetUsers
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Erro a excluir utilizador.");
                }
            }

            // Salva as alterações na base de dados
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizadoresExists(int id)
        {
            return _context.Utilizadores.Any(e => e.Id == id);
        }
    }
}
