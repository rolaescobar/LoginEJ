using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginMVC.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LoginMVC.Controllers
{
    public class AccesoController : Controller
    {
        private readonly LoginContext _context;

        public AccesoController(LoginContext context)
        {
            _context = context;
        }

        // GET: /Acceso/Login
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir al inicio
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Acceso/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string nickname, string contrasena)
        {
            // Buscar el usuario en la base de datos por Nickname y Contrasena
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Nickname == nickname && u.Contrasena == contrasena);

            if (usuario != null)
            {
                // 1. Crear las credenciales (Claims) del usuario
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nickname),
                    new Claim("IdUsuario", usuario.IdUsuario.ToString())
                };

                // 2. Crear la identidad y la cookie de autenticación
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // 3. Redirigir al Home o página principal
                return RedirectToAction("Index", "Home");
            }

            // Si las credenciales son incorrectas
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View();
        }

        // GET: /Acceso/Salir
        public async Task<IActionResult> Salir()
        {
            // Cerrar sesión borrando la cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }
    }
}