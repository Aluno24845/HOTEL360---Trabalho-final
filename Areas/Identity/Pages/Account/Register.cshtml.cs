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
using HOTEL360___Trabalho_final.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)  {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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


            // retirado a referência a 'autenticadores' externos
            //   ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            // os dados recebidos são válidos?
            if (ModelState.IsValid)
            {

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // estamos, aqui, a verdadeiramente guardar os dados
                // da autenticação na base de dados
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // houve sucesso na criação da conta de autenticação
                    _logger.LogInformation("O utilizador criou uma nova conta com password.");

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
                        $"Por favor, confirme a sua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");



                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegistoConfirmado", new { email = Input.Email, returnUrl = returnUrl });
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
