using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using SmartAdmin.WebUI.Authorization;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Services;
using System;
using System.Globalization;
using System.IO;

namespace SmartAdmin.WebUI
{
    public class Startup
    {
        private IHostingEnvironment _env;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //// This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.Configure<CookiePolicyOptions>(options =>
        //    {
        //        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        //        options.CheckConsentNeeded = context => true;
        //        options.MinimumSameSitePolicy = SameSiteMode.None;
        //    });

        //    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
        //    services.AddDefaultIdentity<IdentityUser>()
        //        .AddDefaultUI(UIFramework.Bootstrap4)
        //        .AddEntityFrameworkStores<ApplicationDbContext>();

        //    services.AddMvc(options =>
        //        {
        //            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        //            options.Filters.Add(new AuthorizeFilter(policy));
        //            options.Filters.Add<ViewBagFilter>();
        //        })
        //        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //        app.UseDatabaseErrorPage();
        //    }
        //    else
        //    {
        //        app.UseExceptionHandler("/Home/Error");
        //        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //        app.UseHsts();
        //    }

        //    app.UseHttpsRedirection();
        //    app.UseStaticFiles();
        //    app.UseCookiePolicy();
        //    app.UseAuthentication();

        //    app.UseMvc(routes =>
        //    {
        //        routes.MapRoute(
        //            "default",
        //            "{controller=Intel}/{action=Introduction}/{id?}");
        //    });
        //}

        public void ConfigureServices(IServiceCollection services)
        {
            var webRoot = _env.WebRootPath;
            Directory.CreateDirectory(Path.Combine(webRoot, "assets\\"));
            services.AddSingleton<IFileProvider>(
                           new PhysicalFileProvider(
                             Path.Combine(webRoot, "assets\\")));

            services.AddDbContext<ApplicationDbContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlServer(Configuration.GetConnectionString("AZainBMS-ConnectionString"));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(delegate (IdentityOptions opt)
            {
                opt.Password.RequiredLength = 4;
                opt.Password.RequireDigit = false;
                opt.Password.RequiredUniqueChars = 2;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddClaimsPrincipalFactory<MyUserClaimsPrincipalFactory>();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddDistributedMemoryCache();
            services.ConfigureApplicationCookie(o => o.Events = new CookieAuthenticationEvents());
            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromDays(10));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.SlidingExpiration = false;
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Expiration = TimeSpan.FromDays(14);
                    options.Cookie.IsEssential = true;
                    options.Cookie.Name = ".timeout";
                });

            services.AddScoped<IAuthorizationHandler, AppAuthorizationHandler>();
            services.AddMvc(options => options.Filters.Add<ViewBagFilter>());
            WkHtmlToPdf.Preload(_env);
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            CultureInfo[] supportedCultures = new CultureInfo[1]
            {
                new CultureInfo("ar-EG")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ar-EG"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(delegate (IRouteBuilder routes)
            {
                routes.MapRoute("ControllerName", "{controller}/{action=Index}/{id?}");
                routes.MapRoute("Login", "{controller=Account}/{action=Login}");
            });
        }
    }
}
