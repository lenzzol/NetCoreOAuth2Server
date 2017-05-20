using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Serilog;
using OAuth2Server.Security.Configuration;
using OAuth2Server.Security.Identity.Stores;
using OAuth2Server.Security.Identity;
using OAuth2Server.Data.Context;
using OAuth2Server.Data.Models.Entity;
using OAuth2Server.Data.Repositories;
using OAuth2Server.Security.Managers;
using Microsoft.AspNetCore.Identity;
using OAuth2Server.Security.Identity.Protection;
using OAuth2Server.Email.Configuration;
using Microsoft.AspNetCore.HttpOverrides;
using OAuth2Server.App.Authorization.Configuration;
using OAuth2Server.App.Authorization.UI;

namespace OAuth2Server.Authorization
{
    public class Startup
    {
        private readonly IHostingEnvironment environment;

        public Startup(IHostingEnvironment env)
        {
            this.environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            this.Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, @"Logs\log-{Date}.txt"))
                .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var cert = new X509Certificate2(Path.Combine(this.environment.ContentRootPath, "oauth_sign.pfx"), "abc234"); // put own signed certificate here
            var appSettingsSection = this.Configuration.GetSection("Settings");

            services.AddDbContext<ClientDbContext>(options =>
                options.UseSqlServer(this.Configuration.GetConnectionString("ClientDataStore")));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthServerDataProtectionProvider, AuthServerDataProtectionProvider>();

            services.AddIdentity<AuthUser, AuthRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddTokenProvider<AuthServerTokenProvider<AuthUser>>(TokenOptions.DefaultProvider)
                .AddUserStore<AuthUserStore>()
                .AddRoleStore<AuthRoleStore>();

            // for the UI
            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddRazorOptions(razor =>
                {
                    razor.ViewLocationExpanders.Add(new CustomViewLocationExpander());
                });

            // App Services
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });

            services.Configure<AuthAppSettings>(appSettingsSection);
            services.Configure<AuthServerSettings>(this.Configuration.GetSection("AuthorizationServer"));
            services.Configure<EmailServiceSetup>(this.Configuration.GetSection("EmailService"));
            services.AddTransient<IProfileService, IdentityProfileService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IStatusRepository, StatusRepository>();
            services.AddTransient<IClientRepository, ClientRepository>();
            // services.AddTransient<IClientEmailService, SomeMailService>(); // TODO
            services.AddTransient<IAuthUserManager, AuthUserManager>();

            services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/ui/login";
                options.UserInteraction.LogoutUrl = "/ui/logout";
                options.UserInteraction.ConsentUrl = "/ui/consent";
                options.UserInteraction.ErrorUrl = "/ui/error";
            })
                .AddSigningCredential(cert)
                .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResources())
                .AddInMemoryApiResources(IdentityConfig.GetApiResources())
                //.AddInMemoryScopes(Scopes.Get())
                .AddInMemoryClients(IdentityConfig.GetClients(appSettingsSection["RedirectUri"], appSettingsSection["LogoutRedirectUri"]))
                .AddAspNetIdentity<AuthUser>()
                .AddProfileService<IdentityProfileService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            loggerFactory.AddSerilog();

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseIdentity();
            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
}
