using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AzUtil.Core;

namespace Authzilla
{
    public class Startup
    {
        public Startup(IConfiguration configuration) { _configuration = configuration; }
        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Settings>(_configuration.GetSection("Settings"));
            Settings settings = _configuration.GetSection("Settings").Get<Settings>();
            var providerSwitch = _configuration.GetValue("Provider", ""); //This is captured by the command line switch when running 'dotnet ef command' for migrations
            DatabaseType dbType; if (providerSwitch == "") dbType = settings.Database.Type; else dbType = providerSwitch.ToEnum<DatabaseType>(DatabaseType.SQLite);
            string migrationAssemblyProject = "authzilla." + dbType.ToString().ToLower(); 
            services.AddDbContext<AppDbContext>(options => _ = dbType switch
            {
                DatabaseType.SQLite => options.UseSqlite(settings.Database.ConnectionString, x => x.MigrationsAssembly(migrationAssemblyProject)).UseLowerCaseNamingConvention(),
                DatabaseType.PostgreSQL => options.UseNpgsql(settings.Database.ConnectionString, x => x.MigrationsAssembly(migrationAssemblyProject)).UseLowerCaseNamingConvention(),
                DatabaseType.MSSQL => options.UseSqlServer(settings.Database.ConnectionString, x => x.MigrationsAssembly(migrationAssemblyProject)).UseLowerCaseNamingConvention(),
                _ => throw new NotImplementedException(),
            });
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();            
            services.AddSingleton<ICorsPolicyService>((container) => new DefaultCorsPolicyService(container.GetRequiredService<ILogger<DefaultCorsPolicyService>>()) { AllowAll = true });
            var clients = _configuration.GetSection("InternalClients").Get<InternalClient[]>().Select(c => new Client
            {
                ClientId = c.Username,
                ClientSecrets = { new Secret(c.Password.Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = c.Scopes
            }).ToList();
            clients.Add(new Client
            {
                ClientId = "interactive", //for regular users
                AllowedGrantTypes = GrantTypes.Code, //Grant Type Code is currently the most secure grant type
                RequirePkce = true, //Adding PKCE makes this the most secure login up to date! (March 2021)
                RequireClientSecret = false,
                AllowedCorsOrigins = { "http://localhost:4200" },
                RedirectUris = { "http://localhost:4200/signin-callback", "http://localhost:4200/assets/silent-callback.html" },
                FrontChannelLogoutUri = "http://localhost:4200/signout-oidc",
                PostLogoutRedirectUris = { "http://localhost:4200/signout-callback" },
                //AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "myapi" },
                RequireConsent = false,
                AccessTokenLifetime = 600
            });
            var iS4Builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = false; // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.UserInteraction.LoginUrl = "/Identity/Account/Login";
            })
            .AddOperationalStore<PersistedGrantDbContext>(options =>
            {
                options.ConfigureDbContext = builder =>
                {
                    if (dbType == DatabaseType.SQLite) builder.UseSqlite(settings.Database.ConnectionString, x => x.MigrationsAssembly(migrationAssemblyProject)).UseLowerCaseNamingConvention();
                    if (dbType == DatabaseType.PostgreSQL) builder.UseNpgsql(settings.Database.ConnectionString, x => x.MigrationsAssembly(migrationAssemblyProject)).UseLowerCaseNamingConvention();
                    if (dbType == DatabaseType.MSSQL) builder.UseSqlServer(settings.Database.ConnectionString, x => x.MigrationsAssembly(migrationAssemblyProject)).UseLowerCaseNamingConvention();
                };
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            })
            .AddInMemoryIdentityResources(new IdentityResource[] { new IdentityResources.OpenId(), new IdentityResources.Email(), new IdentityResources.Profile() })
            .AddInMemoryApiScopes(new ApiScope[] { new ApiScope("myapi"), new ApiScope("m2mapi") })
            .AddInMemoryClients(clients)
            .AddDeveloperSigningCredential(); // not recommended for production - you need to store your key material somewhere secure            

            
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    //options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else
            {
                //app.UseExceptionHandler("/Error");
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.Headers.Add("Application-Error", error.Error.Message);
                            context.Response.Headers.Add("access-control-expose-headers", "Application-Error");
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
                });
                //app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };


            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();

            // ref: https://github.com/aspnet/Docs/issues/2384
            app.UseForwardedHeaders(forwardOptions);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});

            //app.UseStaticFiles();

            //app.UseRouting();
            //app.UseIdentityServer();
            //app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapDefaultControllerRoute();
            //});
        }
    }
}
