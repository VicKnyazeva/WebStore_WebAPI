using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WebStore.DAL.Context;
using WebStore.Services.Data;
using WebStore.Domain.Entities.Identity;
using WebStore.Services.Services.InCookies;
using WebStore.Services.Services.InMemory;
using WebStore.Services.Services.InSQL;
using WebStore.Interfaces.Services;

namespace WebStore
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var database_type = Configuration["Database"];

            switch (database_type)
            {

            case "SqlServer":
                services.AddDbContext<WebStoreDB>(opt =>
                    opt.UseSqlServer(Configuration.GetConnectionString(database_type)));
                break;

            case "Sqlite":
                services.AddDbContext<WebStoreDB>(opt =>
                    opt.UseSqlite(Configuration.GetConnectionString(database_type),
                        o => o.MigrationsAssembly("WebStore.DAL.Sqlite")));
                break;

            case "InMemory":
                services.AddDbContext<WebStoreDB>(opt => opt.UseInMemoryDatabase("WebStore.db"));
                break;

            default: 
                throw new InvalidOperationException($"��� �� {database_type} �� ��������������");
            }




            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<WebStoreDB>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(opt =>
            {
#if DEBUG
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 3;
                opt.Password.RequiredUniqueChars = 3;
#endif
                opt.User.RequireUniqueEmail = false;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

                opt.Lockout.AllowedForNewUsers = false;
                opt.Lockout.MaxFailedAccessAttempts = 10;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            });

            services.ConfigureApplicationCookie(opt =>
            {
                opt.Cookie.Name = "WebStore";
                opt.Cookie.HttpOnly = true;
                opt.ExpireTimeSpan = TimeSpan.FromDays(10);

                opt.LoginPath = "/Account/Login";
                opt.LogoutPath = "/Account/Logout";
                opt.AccessDeniedPath = "/Account/AccessDenied";

                opt.SlidingExpiration = true;
            });

            services.AddTransient<WebStoreDbInitializer>();

            services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();
            services.AddScoped<IProductData, SqlProductData>();
            services.AddScoped<ICartService, InCookiesCartService>();
            services.AddScoped<IOrderService, SqlOrderService>();

            services.AddControllersWithViews().
                AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseStatusCodePagesWithRedirects("~/home/Status/{0}");
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}