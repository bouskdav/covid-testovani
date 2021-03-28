using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services
{
    public class LanguageManager
    {
        public static List<Language> availableLanguages = new List<Language> {
            new Language("čeština", "cs"),
            new Language("english", "en"),
            new Language("русский", "ru")
        };

        private readonly HttpContextAccessor _httpContext;

        public LanguageManager(HttpContextAccessor httpContext)
        {
            this._httpContext = httpContext;
        }

        public static bool IsLanguageAvailable(string lang)
        {
            return availableLanguages.Any(a => a.LanguageCultureName.Equals(lang));
        }

        public static string GetDefaultLanguage()
        {
            return availableLanguages[0].LanguageCultureName;
        }

        //public void SetLanguage(string lang)
        //{
        //    if (!IsLanguageAvailable(lang))
        //        lang = GetDefaultLanguage();

        //    var cultureInfo = new CultureInfo(lang);

        //    // TODO - nastavit věci ohledně cultureinfo
        //    //cultureInfo.NumberFormat.CurrencySymbol = "BLA";

        //    Thread.CurrentThread.CurrentUICulture = cultureInfo;
        //    //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);

            
        //    HttpCookie langCookie = new HttpCookie("rap-culture", lang);

        //    langCookie.Expires = DateTime.Now.AddYears(1);

        //    Context.Response.Cookies.Add(langCookie);
        //}

        public void SetLanguage(CultureInfo lang)
        {
            Thread.CurrentThread.CurrentUICulture = lang;
        }

        //public static bool IsDefaultLanguageSet()
        //{
        //    string defaultLanguage = GetDefaultLanguage();

        //    HttpCookie langCookie = HttpContext.Request.Cookies["rap-culture"];

        //    if (langCookie != null)
        //        return langCookie.Value == defaultLanguage;

        //    return true;
        //}
    }

    public class Language
    {
        public Language() { }

        public Language(string name, string culture)
        {
            this.LanguageFullName = name;
            this.LanguageCultureName = culture;
        }

        public string LanguageFullName { get; set; }

        public string LanguageCultureName { get; set; }
    }
}
