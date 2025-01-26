
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Teamo.Core.Interfaces;
using Teamo.Infrastructure.Data;
using TeamoWeb.API.Middleware;

namespace TeamoWeb.API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            // Registers the database context with the DI container
            services.AddDbContext<AppDbContext>(opt => 
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            // Register services with the DI container
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:3000");
                });
            });

            return services;
        }
    }
}
