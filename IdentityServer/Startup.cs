using Flurl.Http;
using Flurl.Http.Configuration;
using IdentityServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServer
{

    public class UntrustedCertClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
        }
    }
    public class Startup
    {
        public static IConfiguration _configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(config => // Aguega una BD
            {
                //config.UseSqlServer(connectionString);
                config.UseInMemoryDatabase("Memory"); // Agrega la BD a memoria
            });

            services.AddIdentity<IdentityUser, IdentityRole>(config => { // Registra los servicios o infraestructura
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders(); // Provee un token por default

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "UCAB.Cookie"; // Nombre de la Cookie
                config.LoginPath = "/Auth/Login"; // Ruta de autenticación
                config.LogoutPath = "/Auth/Logout"; // Ruta de cierre de sesión
            });

            var migrationAssembly = typeof(Startup).Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                //})
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                //})
                .AddInMemoryApiResources(Configuration.GetApis())
                .AddInMemoryClients(Configuration.GetClients())
                .AddInMemoryApiScopes(Configuration.GetScopes())
                .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                .AddDeveloperSigningCredential(); // Para crear una firma para el token

            FlurlHttp.ConfigureClient(_configuration["appSettings:URLBANNERRESTAPI"], cli => cli
            .WithHeaders(new
            {
                Accept = "application/json"
            })
            .WithBasicAuth(_configuration["appSettings:USERBANNERSERVICE"], _configuration["appSettings:PASSWORDBANNERSERVICE"])
            .Settings.HttpClientFactory = new UntrustedCertClientFactory()
            );


            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            // Eso es para los problemas de Chrome con las cookies en http
            if(env.IsDevelopment())
            {
                app.UseCookiePolicy(new CookiePolicyOptions()
                {
                    MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax
                });
            }

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
