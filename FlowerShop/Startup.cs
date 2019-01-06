using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Braintree;

namespace FlowerShop
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
            services.AddDbContext<ApplicationDbContext>(options =>
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
                options.UseInMemoryDatabase("Default")
                );

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender>((iServiceProvider) => new EmailSender(Configuration.GetValue<string>("SendGrid.ApiKey")));
            services.AddTransient<IBraintreeGateway>((IServiceProvider) => new BraintreeGateway(
                Configuration.GetValue<string>("Braintree.Environment"),
                Configuration.GetValue<string>("Braintree.MerchantID"),
                Configuration.GetValue<string>("Braintree.PublicKey"),
                Configuration.GetValue<string>("Braintree.PrivateKey")
                ));

            services.AddTransient<SmartyStreets.USStreetApi.Client>((iSP) =>
            {
                SmartyStreets.ClientBuilder clientBuilder = new SmartyStreets.ClientBuilder(
                    Configuration.GetValue<string>("SmartyStreets.AuthId"),
                    Configuration.GetValue<string>("SmartyStreets.AuthToken")
                );
                return clientBuilder.BuildUsStreetApiClient();
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //var roleManager = services.GetService<RoleManager<IdentityRole>>();
            //if (!roleManager.Roles.Any(x => x.Name == "Administrator"))
            //{
            //    roleManager.CreateAsync(new IdentityRole("Administrator")).Wait();
            //}
        }
    }
}
