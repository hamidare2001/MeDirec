using MeDirect.ExchangeApi.Entities;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Middlewares;
using MeDirect.ExchangeApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace MeDirect.ExchangeApi
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

            services.AddControllers();

            var connectionString = Configuration.GetConnectionString("MeDirectDbContext");
            services.AddDbContext<MeDirectDbContext>(o => o.UseSqlServer(connectionString));


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MeDirect.ExchangeApi", Version = "v1" });
            });

            services.AddHttpClient();

            // Register the RedisCache service
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection("Redis")["ConnectionString"];
            });


            services.AddScoped<ICacheProvider, CacheProvider>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRateApi, RateApi>();
            services.AddScoped<IExchangeService, ExchangeExchangeService>();
            services.AddScoped<ICacheUserService, CacheUserService>();

            services.AddScoped<ICachingRentService, CachingRentService>();


            services.AddScoped<IHttpClient, HttpClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceScopeFactory scopeFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medirect.RateApi v1"));

                scopeFactory.SeedData();
            }

            app.UseCaching();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();
             

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
