using API.Extensions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Teamo.Core.Entities.Identity;
using Teamo.Infrastructure.Data;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Logging.AddAzureWebAppDiagnostics();

builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// Use Swagger API Documentation
app.UseSwagger();

// Set Swagger UI as the root path
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Teamo API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root ("/")
});

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGroup("api").MapIdentityApi<User>();

// Create a scope and call the service manually
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
var userManager = services.GetRequiredService<UserManager<User>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    // Migrate changes to the database
    await applicationDbContext.Database.MigrateAsync();

    // Seed the database with data
    await ApplicationDbContextSeed.SeedAsync(applicationDbContext, userManager, roleManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "A message occured during migration");
}

app.Run();
