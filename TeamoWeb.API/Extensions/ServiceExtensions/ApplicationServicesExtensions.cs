using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Infrastructure.Data;
using Teamo.Infrastructure.Services;

namespace TeamoWeb.API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            // Registers the database context with the DI container
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            // Registers app services
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IFieldService, FieldService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUploadService, UploadService>();

            // Initialize Firebase App
            var credential = GoogleCredential.FromFile(config["Firebase:FirebaseSDKPath"]);
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }

            // Register Firebase services
            services.AddSingleton(provider =>
            {   
                return StorageClient.Create(credential);
            });

            // Register redis server
            services.AddSingleton<IConnectionMultiplexer>(redisConfig =>
            {
                var connectionString = config.GetConnectionString("Redis") 
                    ?? throw new Exception("Failed to get redis connection string");
                var configuration = ConfigurationOptions.Parse(connectionString, true);
                return ConnectionMultiplexer.Connect(configuration);
            });

            // Register cache service
            services.AddSingleton<IApiResponseCacheService, ApiResponseCacheService>();


            services.AddDataProtection();

            services.AddRouting(opt => opt.LowercaseUrls = true);

            services.AddSwaggerGen(c =>
{
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Teamo API", Version = "v1" });

                // Add Bearer token support
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                // Add security scheme in OpenAPI
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Register services with the DI container
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod()
                        .WithOrigins(config["ClientApp"]);
                });
            });

            return services;
        }
    }
}
