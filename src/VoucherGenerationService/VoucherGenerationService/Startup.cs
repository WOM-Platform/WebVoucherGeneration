using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace VoucherGenerationService {

    public class Startup {

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();

            services.AddSession(conf => {
                conf.IdleTimeout = TimeSpan.FromMinutes(15);
                conf.Cookie.Domain = Environment.GetEnvironmentVariable("SELF_HOST");
                conf.Cookie.IsEssential = true;
                conf.Cookie.Name = "WomGeneratorSession";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_BASEPATH");
            if(!string.IsNullOrWhiteSpace(basePath)) {
                app.UsePathBase(new PathString(basePath));
            }

            app.UseSession();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

    }

}
