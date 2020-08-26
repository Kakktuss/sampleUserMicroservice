using System;
using System.IO;
using System.Reflection;
using System.Text;
using Autofac;
using BuildingBlock.Bus.Abstractions.Nats.Events;
using BuildingBlock.Bus.Nats;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserApplication;
using UserInfrastructure.EntityFrameworkDataAccess;

namespace UserApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer();

            services.AddDbContext<UserContext>((serviceProvider, optionsBuilder) =>
            {
                optionsBuilder
                    .UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString"));

                optionsBuilder.UseInternalServiceProvider(serviceProvider);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "User api", Version = "v1"});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = "https://dev-q74vz31h.eu.auth0.com/";
                options.Audience = "http://api.seo-reborn.com/";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("UCNyMRctIR3CIqqwKodTNqSTRWuyfSfYL0L-7fQ-46P_dZpQi-oT_ghQdV-sqE3X"))
                };
                options.RequireHttpsMetadata = false;
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 1);

                options.ReportApiVersions = true;
            });

            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new MediatorAutofacModule());

            // builder.RegisterModule(new RepositoryAutofacModule());

            builder.RegisterModule(new IntegrationEventBusAutofacModule());

            builder.RegisterModule(new NatsBusesAutofacModule(Configuration
                .GetSection("Bus")
                .GetSection("Nats")["ConnectionString"], Configuration
                .GetSection("Bus")
                .GetSection("Nats").GetSection("IsSecureConnection").Get<bool>()));

            builder.RegisterBuildCallback(container =>
            {
                var eventBus = container.Resolve<INatsIntegrationEventBus>();

                /*
                 *  eventBus.Subscribe<StoreLicenseAddedIntegrationEvent, 
                     StoreLicenseAddedIntegrationEventHandler>("store.shopify.license", "created");
                 */
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "License api"); });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}