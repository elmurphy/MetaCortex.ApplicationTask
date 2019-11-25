using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaCortex.ApplicationTask.Manager;
using MetaCortex.ApplicationTask.Manager.Interfaces;
using MetaCortex.ApplicationTask.Process;
using MetaCortex.ApplicationTask.Process.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MetaCortex.ApplicationTask
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
            services.AddDistributedMemoryCache(); // Development cache used, on product mode sqlcache recommended.
            services.AddControllersWithViews();


            services.AddScoped<ICacheManager, CacheManager>(); // Cache dependency included
            services.AddScoped<IExchangeRateManager, ExchangeRateManager>(); // ExchangeManager dependency included


            services.AddSingleton<IXmlRequestProcess>(x => new XmlRequestProcess(x.GetRequiredService<IConfiguration>())); // XmlRequestProcess dependency included(as singleton)
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
