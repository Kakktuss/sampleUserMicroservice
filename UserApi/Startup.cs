using System;
using System.IO;
using System.Reflection;
using System.Text;
using Autofac;
using BuildingBlock.Bus.Abstractions.Nats.Events;
using BuildingBlock.Bus.Stan;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using Prometheus.DotNetRuntime;
using Prometheus.SystemMetrics;
using Prometheus.SystemMetrics.Collectors;
using Serilog;
using STAN.Client;
using UserApi.Authorization;
using UserApplication;
using UserApplication.EntityFrameworkDataAccess;
using UserApplication.IntegrationEvents.Events.Country;
using UserApplication.IntegrationEvents.Handlers.Country;

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
            // Database services
            services.AddEntityFrameworkSqlServer();

            services.AddDbContextPool<UserContext>((serviceProvider, optionsBuilder) =>
            {
                optionsBuilder
                    .UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString"));

                optionsBuilder.UseInternalServiceProvider(serviceProvider);
            });

            // Swagger service
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "User api", Version = "v1"});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // Authorization services
            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = Configuration.GetSection("ThirdParty").GetSection("Auth0")["TenantName"];
                options.Audience = Configuration.GetSection("ThirdParty").GetSection("Auth0")["Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = "https://seo-reborn.com/roles"
                };
                options.RequireHttpsMetadata = false;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("user:self_read", builder => builder.RequirePermission("user:self_read"));
                
                options.AddPolicy("user:self_delete", builder => builder.RequirePermission("user:self_delete"));
                
                options.AddPolicy("user:self_update", builder => builder.RequirePermission("user:self_update"));
            });

            // Cors services
            services.AddCors(options =>
            {
                options.AddPolicy("developerPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .AllowAnyHeader();
                });
                
                options.AddPolicy("prodPolicy", builder =>
                {
                    builder.WithOrigins("https://seo-reborn.com")
                        .WithMethods("GET", "DELETE", "POST")
                        .AllowCredentials()
                        .AllowAnyHeader();
                });
                
                options.AddPolicy("preProdPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .AllowAnyHeader();
                    
                    builder.WithOrigins("https://seo-reborn.com")
                        .WithMethods("GET", "DELETE", "POST")
                        .AllowCredentials()
                        .AllowAnyHeader();
                });
            });
            
            // Api versioning services
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 1);

                options.ReportApiVersions = true;
            });
            
            // Health checks services
            services.AddHealthChecks();

            // Metrics services
            services.AddSystemMetrics(false);
            services.AddSystemMetricCollector<CpuUsageCollector>();
            services.AddSystemMetricCollector<MemoryCollector>();
            services.AddSystemMetricCollector<NetworkCollector>();
            services.AddSystemMetricCollector<LoadAverageCollector>();
            
            // Caching services
            services.AddResponseCaching();

            services.AddControllers();
        }
        
        public virtual void ConfigureMetrics(IApplicationBuilder app)
        {
            var callsCounter = Metrics.CreateCounter("request_total", "Counts the requests to the Country API endpoints", new CounterConfiguration()
            {
                LabelNames = new[] { "method", "endpoint" }
            });

            app.Use((context, next) =>
            {
                callsCounter.WithLabels(context.Request.Method, context.Request.Path);

                return next();
            });
            
            IDisposable collector = DotNetRuntimeStatsBuilder
                .Customize()
                .WithContentionStats()
                .WithJitStats()
                .WithThreadPoolSchedulingStats()
                .WithThreadPoolStats()
                .WithGcStats()
                .WithExceptionStats()
                .StartCollecting();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new RepositoryAutofacModule(Configuration.GetConnectionString("DefaultConnectionString")));

            builder.RegisterModule(new ServiceAutofacModule());
            
            builder.RegisterModule(new IntegrationEventBusAutofacModule());

            var options = StanOptions.GetDefaultOptions();
            options.NatsURL = Configuration
                .GetSection("Bus")
                .GetSection("Nats")["Url"];

            var appName = Configuration
                .GetSection("Bus")
                .GetSection("Nats")["AppName"];

            appName = $"{appName}_{Guid.NewGuid()}";
            
            builder.RegisterModule(new StanBusesAutofacModule(Configuration
                    .GetSection("Bus")
                    .GetSection("Nats")["ClusterName"], 
                appName, 
                options));

            builder.RegisterBuildCallback(container =>
            {
                var eventBus = container.Resolve<IStanIntegrationEventBus>();
                
                var options = StanSubscriptionOptions.GetDefaultOptions();
                options.StartWithLastReceived();
                options.ManualAcks = true;
                options.AckWait = 10000;
                options.DurableName = "user_microservice";

                eventBus.Subscribe<CountryCreatedIntegrationEvent, CountryCreatedIntegrationEventHandler>(
                    "country", "created", "user_microservice", options);
                
                eventBus.Subscribe<CountryDeletedIntegrationEvent, CountryDeletedIntegrationEventHandler>(
                    "country", "deleted", "user_microservice", options);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User authentication api");
                });
            }

            // Configure Routing
            app.UseRouting();

            // Configure API Versioning
            app.UseApiVersioning();
            
            // Configure Authentication and Authorization
            app.UseAuthentication();

            app.UseAuthorization();
            
            // Configure Prometheus Metrics
            app.UseMetricServer();

            app.UseHttpMetrics();
            
            ConfigureMetrics(app);
            
            // Configure serilog to log requests
            app.UseSerilogRequestLogging();

            // Configure response caching
            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}