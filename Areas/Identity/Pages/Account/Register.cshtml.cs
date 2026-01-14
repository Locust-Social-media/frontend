// Licensed to the .NET Foundation under one or more agreements.
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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace Locust.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IConfiguration config)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _config = config;
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
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated == true)
                return Redirect("/Welkom");

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var hasher = new PasswordHasher<object>();
            var hashedPassword = hasher.HashPassword(null, Input.Password);

            var cs = _config.GetConnectionString("MySqlConnection");

            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();
            await using var tx = await conn.BeginTransactionAsync();

            try
            {
                // 1) Insert (tijdelijke id, echte komt hierna)
                await using (var cmd = new MySqlCommand(@"
            INSERT INTO users (id, email, password, role)
            VALUES ('TEMP', @email, @password, 'gebruiker');
        ", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@email", Input.Email);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    await cmd.ExecuteNonQueryAsync();
                }

                // 2) Auto increment nummer ophalen
                long seq;
                await using (var cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID();", conn, tx))
                {
                    seq = Convert.ToInt64(await cmd2.ExecuteScalarAsync());
                }

                // 3) Definitieve ID zetten
                var newId = $"Loc_user_id_{seq}";
                await using (var cmd3 = new MySqlCommand(@"
            UPDATE users
            SET id = @id
            WHERE seq = @seq;
        ", conn, tx))
                {
                    cmd3.Parameters.AddWithValue("@id", newId);
                    cmd3.Parameters.AddWithValue("@seq", seq);
                    await cmd3.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();

                // Succes → naar login
                return RedirectToPage("./Login", new { returnUrl });
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Deze email is al geregistreerd.");
                return Page();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
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
