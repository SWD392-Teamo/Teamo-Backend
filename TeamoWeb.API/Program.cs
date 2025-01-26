using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Teamo.Core.Entities.Identity;
using Teamo.Infrastructure.Data;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Create a scope and call the service manually
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
var userManager = services.GetRequiredService<UserManager<User>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    await applicationDbContext.Database.MigrateAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "A message occured during migration");
}

app.Run();
