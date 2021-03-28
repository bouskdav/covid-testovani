using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using FiremniTestovani.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using FiremniTestovani.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using FiremniTestovani.Data.DbContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Extensions;

namespace FiremniTestovani.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vyplňte email")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Vyplňte heslo")]
            [StringLength(100, ErrorMessage = "{0} musí být dlouhé mezi {2} a {1} znaky.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Heslo")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Heslo znovu")]
            [Compare("Password", ErrorMessage = "Hesla se neshodují.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, Data.Enums.Roles.Basic.ToString());

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

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
