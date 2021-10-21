
using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using WebStore.DAL.Context;

namespace WebStore.WebAPI
{
    public record Startup(IConfiguration Configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var databaseType = Configuration["Database"];

            switch (databaseType)
            {

            case "SqlServer":
                services.AddDbContext<WebStoreDB>(opt =>
                    opt.UseSqlServer(Configuration.GetConnectionString(databaseType)));
                break;

            case "Sqlite":
                services.AddDbContext<WebStoreDB>(opt =>
                    opt.UseSqlite(Configuration.GetConnectionString(databaseType),
                        o => o.MigrationsAssembly("WebStore.DAL.Sqlite")));
                break;

            default:
                throw new InvalidOperationException($"Тип БД {databaseType} не поддерживается");
            }


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebStore.WebAPI", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebStore.WebAPI v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
