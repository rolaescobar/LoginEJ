using Microsoft.AspNetCore.Authentication.Cookies; // <--- 1. Importante para las cookies
using Microsoft.EntityFrameworkCore;
using LoginMVC.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LoginContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LoginConnection")));

// --- 2. AGREGAR ESTO: Configurar la autenticación por Cookies ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login"; // Ruta a la vista de login si no está autenticado
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Tiempo de expiración de sesión
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// --- 3. IMPORTANTE: El orden de estos middlewares es crucial ---
app.UseAuthentication(); // Primero identifica quién es el usuario
app.UseAuthorization();  // Después valida los permisos / [Authorize]

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}") // <--- 4. Cambiado para que inicie directo en el Login (Opcional, puedes dejar Home si prefieres)
    .WithStaticAssets();

app.Run();