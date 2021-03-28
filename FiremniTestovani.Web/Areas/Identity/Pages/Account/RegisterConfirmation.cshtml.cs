using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using FiremniTestovani.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc.Filters;
using FiremniTestovani.Models;
using FiremniTestovani.Data.DbContext;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FiremniTestovani.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayConfirmAccountLink = false;
            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }

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
