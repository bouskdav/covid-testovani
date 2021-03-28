using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FiremniTestovani.Data.Tables;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using FiremniTestovani.Data.DbContext;
using FiremniTestovani.Web.Extensions.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using FiremniTestovani.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FiremniTestovani.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _db;

        public LoginModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Zadejte uživatelské jméno")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Zadejte heslo")]
            [DataType(DataType.Password)]
            [Display(Name = "Heslo")]
            public string Password { get; set; }

            [Display(Name = "Zapamatovat?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Nelze se přihlásit. Účet je uzamčen.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Neplatný pokus o přihlášení.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            int? sourceID;
            ApplicationSource source;

            ApplicationDbContext db = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();

            if (context.HandlerArguments.ContainsKey("source") && context.HandlerArguments["source"] != null)
            {
                sourceID = (int)context.HandlerArguments["source"];

                var dbSource = db.Sources
                    //.Include(i => i.R_DefaultCurrency)
                    .SingleOrDefault(i => i.SourceID == sourceID);

                if (dbSource == null)
                {
                    context.Result = new ContentResult() { Content = $"Této doméně neodpovídá žádná firemní stránka nastavená v tabulce zdrojů!" };

                    return base.OnPageHandlerExecutionAsync(context, next);
                }

                source = new ApplicationSource(dbSource);
            }
            else
            {
                var url = context.HttpContext.Request.GetDisplayUrl();

                var sourcesList = db.Sources
                        //.Include(i => i.R_DefaultCurrency)
                        .ToList();

                //var dbSource = sourcesList
                //    .Where(i => !String.IsNullOrEmpty(i.URL))
                //    .OrderByDescending(i => i.URL.Length)
                //    .FirstOrDefault(i => url.Contains(i.URL));

                // at first try externalURLs
                var dbSource = sourcesList
                    .Where(i => !String.IsNullOrEmpty(i.URL))
                    .OrderByDescending(i => i.URL.Length)
                    .FirstOrDefault(i => url.Contains(i.URL));

                // then internal ones (only if no match)
                if (dbSource == null)
                {
                    dbSource = sourcesList
                        .Where(i => !String.IsNullOrEmpty(i.InternalURL))
                        .OrderByDescending(i => i.InternalURL.Length)
                        .FirstOrDefault(i => url.Contains(i.InternalURL));
                }

                if (dbSource == null)
                {
                    context.Result = new ContentResult() { Content = "Této doméně neodpovídá žádná firemní stránka nastavená v tabulce zdrojů!" };

                    return base.OnPageHandlerExecutionAsync(context, next);
                }

                sourceID = dbSource.SourceID;
                source = new ApplicationSource(dbSource);

                if (context.HandlerArguments.ContainsKey("source"))
                    context.HandlerArguments["source"] = sourceID;
                else
                    context.HandlerArguments.Add("source", sourceID);
            }

            //zdroj.UICulture.NumberFormat.CurrencySymbol = zdroj.Currency.CurrencySymbol;
            //zdroj.UICulture.NumberFormat.CurrencyPositivePattern = 3;
            //zdroj.UICulture.NumberFormat.CurrencyNegativePattern = 8;

            context.HttpContext.Items["source"] = source;
            //new LanguageManager().SetLanguage(zdroj.UICulture);

            return base.OnPageHandlerExecutionAsync(context, next);
        }
    }
}
