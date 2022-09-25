// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using Projeto.Data;
using Projeto.Models;

namespace Projeto.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IEnumerable<Universidade> Universidades { get; set; }
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
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
            Universidades = _context.Universidades;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Dados do estudante (Nome e universidade que frequenta)
            /// </summary>
            public Estudante? Estudante { get; set; }

            /// <summary>
            /// Email do estudante
            /// </summary>
            [EmailAddress]
            [Display(Name = "Email")]
            public string? EmailEstudante { get; set; }

            /// <summary>
            /// Fotografia do estudante
            /// </summary>
            public string? Fotografia { get; set; }

            /// <summary>
            /// Password do estudante
            /// </summary>
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string? PasswordEstudante { get; set; }

            /// <summary>
            /// Confirmar Password do Estudante
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("PasswordEstudante", ErrorMessage = "A password e password de confirmação não são iguais.")]
            public string? ConfirmPasswordEstudante { get; set; }

            //*****Dados da Universidade****//

            //Nome da Universidade
            public Universidade? Universidade { get; set; }

            //Email da Universidade
            [EmailAddress]
            [Display(Name = "Email")]
            public string? EmailUniversidade { get; set; }

            //Password Universidade
            [StringLength(100, ErrorMessage = "The {0} deve ter pelo menos {2} e no máximo {1} carácteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string? PasswordUniversidade { get; set; }

            //Confirmar Password da Universidade
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("PasswordUniversidade", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPasswordUniversidade { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile fotoUser, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                var result = await _userManager.CreateAsync(user);
                //Adicionar tokens a cada usuário
                if (Input.EmailUniversidade != null)
                {
                    await _userStore.SetUserNameAsync(user, Input.EmailUniversidade, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.EmailUniversidade, CancellationToken.None);

                    //adicionar username a tabela
                    user.Name = Input.Universidade.Nome;
                    //adicionar o Id de usuário a tabela Universidade
                    Input.Universidade.UserId = user.Id;

                    result = await _userManager.CreateAsync(user, Input.PasswordUniversidade);
                }
                else if (Input.EmailEstudante != null)
                {
                    await _userStore.SetUserNameAsync(user, Input.EmailEstudante, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.EmailEstudante, CancellationToken.None);
                    //adicionar username a tabela
                    user.Name = Input.Estudante.UserName;
                    //Adicionar o Id do usuário a tabela de usuários
                    Input.Estudante.UserId = user.Id;
                    //imagem de perfil do usuario
                    if (fotoUser == null)
                    {
                        //se não tiverfotografia adicionar uma padrão
                        Input.Estudante.Fotografia = "user.png";
                    }
                    else
                    {
                        if (!(fotoUser.ContentType == "image/jpeg" || fotoUser.ContentType == "image/png"))
                        {
                            // write the error message
                            ModelState.AddModelError("", "Please, if you want to send a file, please choose an image...");
                            // resend control to view, with data provided by user
                            return Page();
                        }
                        else
                        {
                            // define image name
                            Guid g;
                            g = Guid.NewGuid();
                            string imageName = Input.Estudante.UserId + "_" + g.ToString();
                            string extensionOfImage = Path.GetExtension(fotoUser.FileName).ToLower();
                            imageName += extensionOfImage;
                            // add image name to vet data
                            Input.Estudante.Fotografia = imageName;
                        }

                    }
                    result = await _userManager.CreateAsync(user, Input.PasswordEstudante);


                }

                if (result.Succeeded)
                {

                    /*
                     * Caso tudo corra bem colocar os dados do usário "User" ou "Admin" nas suas respectivas
                     * bases de dados de identificação
                     */
                    try
                    {
                        if (Input.EmailUniversidade != null)
                        {
                            //Criar e dar acesso a newsletter da universidade 
                            Newsletter newsletter = new Newsletter();
                            newsletter.UniversidadeFk = Input.Universidade.Id;
                            _context.Add(newsletter);
                            await _context.SaveChangesAsync();

                            //Criar e dar a newsletter a universidade
                            Input.Universidade.NewsletterFk = newsletter.Id; 
                            _context.Add(Input.Universidade);
                            await _context.SaveChangesAsync();
                        }
                        else if (Input.EmailEstudante != null)
                        {
                            _context.Add(Input.Estudante);
                            await _context.SaveChangesAsync();
                        }

                    }
                    catch (Exception)
                    {
                        // if I am here, something bad happened
                        // what I need to do????
                        // 
                        // I must do a Rollback to all process
                        // this mean - delete the user prevously created
                        await _userManager.DeleteAsync(user);
                        // create a message to user
                        ModelState.AddModelError("", "It was impossible to create user. Something wrong happened");
                        // return control to user
                        return Page();
                    }
                    //---------------------------------------------------------------------------------------
                    // save image file to disk
                    // ********************************
                    if (fotoUser != null)
                    {
                        // ask the server what address it wants to use
                        string addressToStoreFile = _webHostEnvironment.WebRootPath;
                        string newImageLocalization = Path.Combine(addressToStoreFile, "Photos//User");
                        // see if the folder 'Photos' exists
                        if (!Directory.Exists(newImageLocalization))
                        {
                            Directory.CreateDirectory(newImageLocalization);
                        }
                        // save image file to disk
                        newImageLocalization = Path.Combine(newImageLocalization, Input.Estudante.Fotografia);
                        using var stream = new FileStream(newImageLocalization, FileMode.Create);
                        await fotoUser.CopyToAsync(stream);
                    }

                    _logger.LogInformation("User created a new account with password.");

                    //Criar roles para cada usuário que se pretende logar
                    //Universidade tem um role de "Admin"
                    //User tem um role de "User"
                    if (Input.EmailUniversidade != null)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else if (Input.EmailEstudante != null)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    if (Input.EmailUniversidade != null)
                    {
                        await _emailSender.SendEmailAsync(Input.EmailUniversidade, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    }
                    else if (Input.EmailEstudante != null)
                    {
                        await _emailSender.SendEmailAsync(Input.EmailEstudante, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    }


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        if (Input.EmailUniversidade != null)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.EmailUniversidade, returnUrl = returnUrl });
                        }
                        else if (Input.EmailEstudante != null)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.EmailEstudante, returnUrl = returnUrl });
                        }
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

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}