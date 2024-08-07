﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using HOTEL360___Trabalho_final.Data;
using HOTEL360___Trabalho_final.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace HOTEL360___Trabalho_final.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel  {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;


        /// <summary>
        /// referência à BD do projeto
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// objeto que contém os dados referentes ao ambiente 
        /// do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;



        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
          ApplicationDbContext context,
          IWebHostEnvironment webHostEnvironment
         )
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     Objeto a ser utilizado para transportar os dados entre a interface e o nosso código
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     Esta variável irá conter o 'destino' a ser aplicado pela aplicação, quando após o 'registo' a aplicação
        /// pretender ser reposicionada na página original
        /// </summary>
        public string ReturnUrl { get; set; }

        // /// <summary>
        // /// se se adicionar as chaves de autenticação por 
        // /// 'providers' externos, aqui serão listados
        // /// por esta variável
        // /// Ver: https://go.microsoft.com/fwlink/?LinkID=532715
        // /// </summary>
        // public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// 'inner class'
        /// Define os atributos a serem enviados/recebidos para/da interface
        /// </summary>
        public class InputModel  {

            /// <summary>
            /// Email do utilizador
            /// </summary>
            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
            [EmailAddress(ErrorMessage = "Escreva um {0} válido, por favor.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            /// Password de acesso ao sistema, pelo utilizador
            /// </summary>
            [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
            [StringLength(20, ErrorMessage = "A {0} tem de ter, pelo menos {2}, e um máximo de {1} caracteres.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            /// Confirmação da password
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "A password e a sua confirmação não coincidem.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            /// Recolhe os dados do Utilizador HOSPEDE
            /// </summary>
            public Hospedes Hospede { get; set; }

            /// <summary>
            /// Representa o ficheiro de avatar enviado pelo utilizador durante o registro.
            /// </summary>
            public IFormFile AvatarFile { get; set; }
        }


        /// <summary>
        /// este método reage ao verbo HTTP GET
        /// </summary>
        /// <param name="returnUrl">o endereço onde 'estávamos'
        ///  quando foi feito o pedido para nos registarmos
        /// </param>
        /// <returns></returns>
        public void OnGet(string returnUrl = null)
        {
            // guarda no atributo 'ReturnUrl' o parâmetro de
            ReturnUrl = returnUrl;

            //  ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            // pq se retirou esta instrução, foi necessário 
            // tornar o nosso método síncrono
        }

        /// <summary>
        /// este método recolhe os dados enviados pelo Utilizador
        /// </summary>
        /// <param name="returnUrl">página a redirecionar,
        ///      após a operação de Registar terminar
        /// </param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // se returnUrl = NULL,
            // somos redirecionado para a raiz da app
            returnUrl ??= Url.Content("~/");

            //vars auxiliares
            string nomeImagem = "";
            bool haImagem = false;

            // há ficheiro?
            if (Input.AvatarFile == null)
            {
                ModelState.AddModelError("",
                   "O fornecimento de um Avatar é obrigatório!");
                return Page();
            }
            else
            {
                // há ficheiro, mas é imagem?
                if (!(Input.AvatarFile.ContentType == "image/png" ||
                       Input.AvatarFile.ContentType == "image/jpeg")
                   )
                {
                    ModelState.AddModelError("",
                   "Tem de fornecer para o Avatar um ficheiro PNG ou JPG!");
                    return Page();
                }
                else
                {
                    // há ficheiro, e é uma imagem válida
                    haImagem = true;
                    // obter o nome a atribuir à imagem
                    Guid g = Guid.NewGuid();
                    nomeImagem = g.ToString();
                    // obter a extensão do nome do ficheiro
                    string extensao = Path.GetExtension(Input.AvatarFile.FileName);
                    // adicionar a extensão ao nome da imagem
                    nomeImagem += extensao;
                    // adicionar o nome do ficheiro ao objeto que
                    // vem do browser
                    Input.Hospede.Avatar = nomeImagem;


                }
            }


            // os dados recebidos são válidos?
            if (ModelState.IsValid)  {

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // estamos, aqui, a verdadeiramente guardar os dados
                // da autenticação na base de dados
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded) {
                    // houve sucesso na criação da conta de autenticação
                    _logger.LogInformation("O utilizador criou uma nova conta com password.");

                    // **********************************************
                    // Vamos atribuir à pessoa que se registou o Role HOSPEDES
                    await _userManager.AddToRoleAsync(user, "Hospedes");
                    // **********************************************


                    // **********************************************
                    // vamos escrever na BD os dados do Hospede
                    // na prática, quero guardar na BD os
                    // dados do atributo 'input.Hospede'
                    // **********************************************

                    // vamos guardar o valor do atributo
                    // que fará a 'ponte' entre a BD
                    // de autenticação e a BD do 'negócio'
                    Input.Hospede.UserId = user.Id;                    

                    try
                    {
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
                            await Input.AvatarFile.CopyToAsync(stream);
                        }
                        // guardar os dados na BD
                        _context.Add(Input.Hospede);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        // há que registar os dados do que
                        // aconteceu mal, para se reparar
                        // o problema

                        // se cheguei aqui é pq não se conseguiu
                        // escrever os dados do Hospede na BD
                        // há que tomar uma decisão sobre o que fazer...

                        // Sugestão:
                        // - guardar os dados da exceção num ficheiro de 'log'
                        //      no disco rígido do servidor
                        // - guardar os dados da exceção numa tabela da BD
                        // - apagar o 'utilizador' criado na linha 154
                        // - notificar a pessoa que está a interagir com a 
                        //      aplicação do sucedido
                        // - redirecionar a pessoa para uma página de erro

                        _logger.LogInformation(ex.ToString());

                        throw;
                    }
                    // **********************************************

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // envia email para o utilizador com o código
                    // de validação do email inserido
                    // SÓ APÓS a aceitação desta tarefa o utilizador pode
                    // entrar na app
                    await _emailSender.SendEmailAsync(Input.Email, "Confirme o seu email",
                        $"Por favor, confirme a sua conta by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
            foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
