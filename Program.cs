using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using VeterinariaSystem.Models;

var builder = WebApplication.CreateBuilder(args);
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuarios/Login";
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Home/Restringido";
    })
    .AddJwtBearer(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            var key = builder.Configuration["TokenJwt:SecretKey"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            };
        }
    );

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole(nameof(enRoles.Administrador)));
    options.AddPolicy(
        "Veterinario",
        policy => policy.RequireRole(nameof(enRoles.Veterinario), nameof(enRoles.Administrador))
    );
    options.AddPolicy(
        "Dueno",
        policy => policy.RequireRole(nameof(enRoles.Dueno), nameof(enRoles.Administrador))
    );
});

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

builder.Services.AddScoped<IRepositorioDueno, RepositorioDueno>();
builder.Services.AddScoped<IRepositorioMascota, RepositorioMascota>();
builder.Services.AddScoped<IRepositorioVeterinario, RepositorioVeterinario>();
builder.Services.AddScoped<IRepositorioTurno, RepositorioTurno>();
builder.Services.AddScoped<IRepositorioConsulta, RepositorioConsulta>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseCors("CorsPolicy");

app.MapControllers();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
