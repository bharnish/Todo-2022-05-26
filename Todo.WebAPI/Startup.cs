using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Todo.Core;
using Todo.Data;
using Todo.Services;
using Todo.Services.Implementations;

namespace Todo.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAutoMapper(GetType().Assembly);
            services.AddSwaggerGen();

            services.AddScoped<IDynamoDBContext, DynamoDBContext>();
            services.AddScoped<IAmazonDynamoDB, AmazonDynamoDBClient>();

            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            services.AddSingleton<IContextParser, ContextParser>();

#if DEBUG
            services.AddScoped<IRepo, NormalRepo>();
#else
            services.AddScoped<IRepo, CryptoRepo>();
#endif
            ScanForDefaultInterface(services, GetType().Assembly);
            ScanForDefaultInterface(services, typeof(ITodoParser).Assembly);
        }

        private void ScanForDefaultInterface(IServiceCollection services, Assembly assembly)
        {
            var types =
                assembly
                    .GetTypes()
                    .Where(t => t.IsClass)
                    .Where(t => GetDefaultInterface(t) != null);

            foreach (var type in types)
            {
                var defaultInterface = GetDefaultInterface(type);
                if (defaultInterface == null) continue;
                
                var isScoped = type.GetInterface(nameof(IScoped)) != null;
                if (isScoped)
                    services.AddScoped(defaultInterface, type);
                else
                    services.AddSingleton(defaultInterface, type);
            }

            Type GetDefaultInterface(Type t) => t.GetInterface("I" + t.Name);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(o => o.SerializeAsV2 = true);
            app.UseSwaggerUI();

            app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                });
            });
        }
    }
}
