using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Rewrite;
using System.Threading.Tasks;
using CursorShops.Models;
using CursorShops.Infrastructure;
using System.Linq;
using System;
using System.Globalization;

namespace CursorShops
{
    public class Startup
    {
        static public IConfigurationRoot Configuration { get; set; }
        static public IHostingEnvironment Environment { get; set; }
        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json").Build();
            Environment = env;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ShopIdentityDbContext>(options => options.UseSqlServer(Configuration["ConnectionStringsAzure:DefaultConnection"]));
            services.AddDbContext<ShopIdentityDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddIdentity<ShopUser, IdentityRole>(opts => {
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = true;
                opts.User.AllowedUserNameCharacters = "ABCDEFGHIJKLMNOPQRSTUWVXYZabcdefghijklmnopqrstuvwxyz1234567890АаБбВвГгДдЕеЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчЩщЪъЫыЬьЭэЮюЯя ";
            }).AddEntityFrameworkStores<ShopIdentityDbContext>();
            services.AddTransient(typeof(IMyDI),typeof(TestDI));

            /*
            //правим баг в net Core 2 - когда вылазит ошибка при подключении в Dependencies сторонней сборки
            services.AddMvc().ConfigureApplicationPartManager(manager =>
            {
                var oldMetadataReferenceFeatureProvider = manager.FeatureProviders.First(f => f is Microsoft.AspNetCore.Mvc.Razor.Compilation.MetadataReferenceFeatureProvider);
                manager.FeatureProviders.Remove(oldMetadataReferenceFeatureProvider);
                manager.FeatureProviders.Add(new Microsoft.AspNetCore.Mvc.Razor.Compilation.ReferencesMetadataReferenceFeatureProvider());
            });
            */
            //устанавливаем max размер параметра формы в байтах (примерно 1 гбайт)
            services.Configure<FormOptions>(p=>p.ValueLengthLimit = 1073741824);
            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //делаем перенаправление с локального url на открытый интернетовский owa для чек-листов
            //RewriteOptions options = new RewriteOptions().AddRedirect("WorkPlace/owa/(.*)", "http://sptest/sites/kadr/$1");
            //app.UseRewriter(options);
            CultureInfo cultureInfo = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseProxyServer();//перехваьываем все запросы для кастомизированной обработки обращений к веб-сервисам и для перенаправления обращений к чек-листам. Обработка в классе ProxyMiddleware.cs
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: null, template: "{controller}/{action}/{id?}", defaults: new { controller = "Main", action = "Index" });
            });
            ShopIdentityDbContext.CreateAdminAccount(app.ApplicationServices, Configuration).Wait();
        }
    }
}
