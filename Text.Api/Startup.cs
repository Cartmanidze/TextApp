using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcText;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Text.Api.Infrastructure.Contexts;
using Text.Api.Infrastructure.Models;
using Text.Api.Infrastructure.Repositories;

namespace Text.Api
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
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddDbContext<TextContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("AppConnectionString");
                options.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            services.AddScoped<ITextFileRepository, TextFileRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Text service started!");
                });
                endpoints.MapGrpcService<TextService>();
            });
        }
    }
}
