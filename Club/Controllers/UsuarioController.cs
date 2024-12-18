﻿using Microsoft.AspNetCore.Mvc;
using Club.Data;
using Club.Models;
using BCrypt.Net; // Necesario para verificar las contraseñas

namespace Club.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar la vista de inicio de sesión
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult Login(string email, string contrasena)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contrasena))
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar el correo y la contraseña.");
                return View();
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "El usuario no existe.");
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena))
            {
                ModelState.AddModelError(string.Empty, "La contraseña es incorrecta.");
                return View();
            }

            // Guardar datos del usuario en la sesión
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetString("UsuarioId", usuario.UsuarioId.ToString());

            return RedirectToAction("Dashboard", "Home");
        }



        // Mostrar mensaje de inicio exitoso
        public IActionResult InicioExitoso()
        {
            return View();
        }

        // Métodos de registro ya existentes (los dejamos tal como están)
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                ModelState.AddModelError("Email", "El correo ya está registrado.");
                return View(usuario);
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return RedirectToAction("RegistroExitoso");
        }


        public IActionResult RegistroExitoso()
        {
            return View();
        }
        public IActionResult Logout()
        {
            // Limpiar sesión
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Usuario");
        }


    }
}
