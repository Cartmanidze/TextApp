using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using GrpcTask;
using Microsoft.EntityFrameworkCore;
using Task.Api.Infrastructure.Contexts;
using Task.Api.Infrastructure.Repositories;

namespace Task.Api
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
            services.AddDbContext<TaskContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("AppConnectionString");
                options.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            services.AddScoped<ITaskFileRepository, TaskFileRepository>();
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
                endpoints.MapGrpcService<TaskService>();
            });
        }
    }
}
