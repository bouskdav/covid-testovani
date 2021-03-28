using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FiremniTestovani.Data.DbContext;
using Postal.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using AspNetCore.SEOHelper;
using FiremniTestovani.Web.Extensions.Filters;
using FiremniTestovani.Data.Tables;
using FiremniTestovani.Web.Models;
using FiremniTestovani.Web.Services.Abstraction;
using ExpressiveAnnotations.MvcCoreUnobtrusive;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using FiremniTestovani.Web.Extensions;
using FiremniTestovani.Data;

namespace FiremniTestovani.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //// add expressive annotations
            //services.AddExpressiveAnnotations();

            // email configuration section
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<SMSSettings>(Configuration.GetSection("SMSSettings"));

            // switch ddb provider based on configuration, default to mysql
            switch (Configuration["FT:DbProvider"])
            {
                case "SqlServer":
                    // database
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(
                            Configuration.GetConnectionString("DefaultConnectionSqlServer"),
                            m => m.MigrationsAssembly("FiremniTestovani.Data.Migrations.SqlServer")));
                    break;
                default:
                    // database
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseMySql(
                            Configuration.GetConnectionString("DefaultConnection"),
                            m => m.MigrationsAssembly("FiremniTestovani.Data")));
                    break;
            }

            // settings service
            services.AddTransient<ISourceDetectionService, Services.SourceDetectionService>();
            services.AddTransient<ISourceSettingsService, Services.SourceSettingsService>();

            // authentication (identity core)
            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add detection services container and device resolver service.
            services.AddDetection();

            // Postal service
            services.AddPostal();

            //// CORS
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("DronoskolaPolicy",
            //        builder =>
            //        {
            //            builder.WithOrigins("https://dronoskola.cz",
            //                                "https://www.dronoskola.cz");
            //        });
            //});

            services.AddSession();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(SelectSourceBasedOnURLAttribute));
                options.Filters.Add(typeof(LogExceptionsAttribute));
            })
            //.AddRazorRuntimeCompilation() // runtime kompilace
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(); // aby fungovaly pøeklady

            services.AddRazorPages();

            // add expressive annotations
            services.AddExpressiveAnnotations();

            // configure cultures
            services.Configure<RequestLocalizationOptions>(options => {
                List<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("cs"),
                    new CultureInfo("en"),
                    new CultureInfo("sr"),
                    new CultureInfo("bg"),
                    new CultureInfo("ro"),
                    new CultureInfo("vi"),
                    new CultureInfo("uk"),
                    new CultureInfo("ru")
                };

                options.DefaultRequestCulture = new RequestCulture("cs");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            // view expander (to override folder)
            //services.Configure<RazorViewEngineOptions>(options =>
            //{
            //    options.ViewLocationExpanders.Add(new ApplicationViewLocationExpander());
            //});

            //Singleton which creates a single instance throughout the application.It creates the instance for the first time and reuses the same object in the all calls.
            //Scoped lifetime services are created once per request within the scope.It is equivalent to a singleton in the current scope.For example, in MVC it creates one instance for each HTTP request, but it uses the same instance in the other calls within the same web request.
            //Transient lifetime services are created each time they are requested.This lifetime works best for lightweight, stateless services.

            // notification services
            services.AddTransient<IMailService, Services.MailService>();
            services.AddTransient<ISMSService, Services.SMSOverSMTPService>();
            services.AddTransient<INotificationService, Services.NotificationService>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dataContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // migrate any database changes on startup (includes initial db creation)
            dataContext.Database.Migrate();

            // seed data
            {
                ContextSeed.SeedRolesAsync(userManager, roleManager).Wait();
                ContextSeed.SeedSuperAdminAsync(userManager, roleManager).Wait();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                // Forward headers
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // SEO helpers
            app.UseXMLSitemap(env.ContentRootPath);
            app.UseRobotsTxt(env.ContentRootPath);

            //// Translation
            //var supportedCultures = new[] { "cs", "en" };
            //var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
            //    .AddSupportedCultures(supportedCultures)
            //    .AddSupportedUICultures(supportedCultures);

            //app.UseRequestLocalization(localizationOptions);
            // Translation -> get default culture from browser
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            // Use Wangkanai detection services
            app.UseDetection();

            app.UseRouting();

            // Use CORS policies
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
