using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindProductByImage.ef;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Session;

namespace FindProductByImage
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connection = Configuration.GetConnectionString("DefaultConnection"); //@"Data Source = .; Initial Catalog = ProductScanDB; Integrated Security = True";
            //var connection = @"Data Source = TANIS\SQLEXPRESS; Initial Catalog = ProductScanDB; Integrated Security = True";
            services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));
            services.AddMvc();

            //services.AddPaging();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(3);
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=ProductDetails}/{action=Index}/{id?}");
            });
        }
    }
}
