using API.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Teamo.Core.Entities.Identity;
using Teamo.Infrastructure.Data;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.UseHttpsRedirection();

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
