using System.IdentityModel.Tokens.Jwt;
using System.IO;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Serilog;
using OAuth2Server.AccountService.Managers;
using OAuth2Server.Data.Context;
using OAuth2Server.Data.Models.Entity;
using OAuth2Server.Data.Repositories;
using OAuth2Server.Email.Configuration;
using OAuth2Server.Security.Identity.Stores;
using OAuth2Server.Security.Managers;
using OAuth2Server.Email.Services;
using Microsoft.AspNetCore.Identity;
using OAuth2Server.Security.Identity.Protection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace OAuth2Server.Api.Apps
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, @"Logs\log-{Date}.txt"))
                .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add EF core
            services.AddDbContext<ClientDbContext>(options =>
                options.UseSqlServer(this.Configuration.GetConnectionString("ClientDataStore")));

            // App services
            services.Configure<AuthServerSettings>(this.Configuration.GetSection("AuthorizationServer"));
            services.Configure<EmailServiceSetup>(this.Configuration.GetSection("EmailService"));
            //services.AddTransient<IClientEmailService, MailService>(); TODO: implement email service
            services.AddTransient<IClientManager, ClientManager>();
            services.AddTransient<IAuthUserManager, AuthUserManager>();
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IStatusRepository, StatusRepository>();

            services.AddScoped<IAuthServerDataProtectionProvider, AuthServerDataProtectionProvider>(); // custom key backing

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

            //Add Cors support to the service
            services.AddCors(options =>
            {
                options.AddPolicy("corsGlobalPolicy",
                    builder => builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                );
            });

            services.AddOptions();

            // Add framework services.
            services.AddMvc(options =>
            {
                //options.Filters.Add(new AuthorizeFilter(guestPolicy));
            }).AddJsonOptions(a => a.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<AuthServerSettings> authSettings)
        {

            loggerFactory.AddSerilog();

            app.UseCors("corsGlobalPolicy");

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            IdentityServerAuthenticationOptions identityServerValidationOptions = new IdentityServerAuthenticationOptions
            {
                Authority = authSettings.Value?.BaseUrl,
                AutomaticAuthenticate = true,
                AllowedScopes = new List<string>() { "accountService" },
                ApiSecret = "accountServiceSecret",
                ApiName = "accountService",
                SupportedTokens = SupportedTokens.Both,
                AutomaticChallenge = true,
            };

            app.UseIdentity();
            app.UseIdentityServerAuthentication(identityServerValidationOptions);
            app.UseMvc();
        }
    }
}
