using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Teamo.Core.Entities.Identity;
using Teamo.Infrastructure.Data;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, 
            IConfiguration config)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
            });

            // Set up aspnet identity
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddAuthorization();
            services.AddIdentityApiEndpoints<User>(opt =>
            {
                //Set you account options here (e.g., Password, Email)
                opt.SignIn.RequireConfirmedAccount = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            })
            .AddRoles<IdentityRole<int>>()
            .AddSignInManager<SignInManager<User>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }
    }
}