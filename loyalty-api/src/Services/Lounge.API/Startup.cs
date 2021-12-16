using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.Extensions;
using Utils;
using Boxed.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;
using Admin.API.Infrastructure.Migrations;
using Serilog;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using API.Filters;

namespace Employee.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            // Init Serilog configuration
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                //.MinimumLevel.Information()
                .CreateLogger();

            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string dbType = "Oracle";
            services.AddMvc(x => x.Filters.Add(typeof(LogFilter))).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            // Register Context
            switch (dbType)
            {
                case "MSSQL":
                    services.AddDbContext<ModelContext>(opts => opts.UseSqlServer(Configuration["ConnectionString:" + dbType]));
                    break;
                case "Oracle":
                    services.AddDbContext<ModelContext>(opts => opts.UseOracle(Configuration["ConnectionString:" + dbType]));
                    break;
                default:
                    services.AddDbContext<ModelContext>(opts => opts.UseSqlServer(Configuration["ConnectionString:" + dbType]));
                    break;
            }

            //redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Helpers.GetConfig("Redis:ConnectionString");
                options.InstanceName = Helpers.GetConfig("Redis:InstanceName");
            });
            //end redis

            services
                .AddCustomCaching()
                .AddCustomOptions(Configuration)
                .AddCorrelationIdFluent()
                .AddCustomRouting()
                .AddResponseCaching()
                .AddCustomResponseCompression()
                .AddCustomStrictTransportSecurity()
                .AddHttpContextAccessor()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddCustomApiVersioning()
                .AddMvcCore()
                .AddApiExplorer()
                .AddAuthorization()
                .AddDataAnnotations()
                .AddJsonFormatters()
                .AddCustomJsonOptions(HostingEnvironment)
                .AddCustomCors(Configuration)
                .AddCustomMvcOptions(HostingEnvironment)
                .Services
                .AddProjectServices()
                .BuildServiceProvider();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.OperationFilter<MyHeaderFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Logging
            loggerFactory.AddSerilog();
            // Pass a GUID in a X-Correlation-ID HTTP header to set the HttpContext.TraceIdentifier.
            app.UseCorrelationId()
            .UseForwardedHeaders()
            .UseResponseCaching()
            .UseResponseCompression()
            .UseCors(CorsPolicyName.AllowAny)
            .UseDeveloperErrorPages()
            // .UseIf(
            //     this.hostingEnvironment.IsDevelopment(),
            //     x => x.UseDeveloperErrorPages())
            .UseStaticFilesWithCacheControl()
            .UseAuthentication()
            .UseIf(
                Configuration["Cors:AllowAll"] == "true",
                x => x.UseCors(CorsPolicyName.AllowAny))
            .UseIf(
                Configuration["Cors:AllowAll"] == "true",
                x => x.UseCors(CorsPolicyName.AllowFrontEnd).UseCors(CorsPolicyName.AllowThirdparty))
            .UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
    /// <summary>
    /// Operation filter to add the requirement of the custom header
    /// </summary>
    public class MyHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "X-UserName",
                In = "header",
                Type = "string",
                Required = false // set to false if this is optional
            });
            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "X-PermissionToken",
                In = "header",
                Type = "string",
                Required = false // set to false if this is optional
            });
        }
    }
}
