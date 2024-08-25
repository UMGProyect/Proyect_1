using Microsoft.AspNetCore.Mvc;
using Proyect_1.ContextBD;
using Proyect_1.Models;
using System.Diagnostics;

namespace Proyect_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //Instancia de sesión.
        private readonly Sesion iniciar_sesion = new Sesion();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult CheckSession()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                return Json(new { isAuthenticated = true });
            }
            else
            {
                return Json(new { isAuthenticated = false });
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Main(User model)
        {
            model.Name = HttpContext.Session.GetString("UserName");
            if (HttpContext.Session.GetString("IsAuthenticated") != "true")
            {
                return RedirectToAction("Login");
            }

            return View(model);
        }
        //Manejo de solicitud POST.
        [HttpPost]
        public IActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                bool User_Authenticator = iniciar_sesion.Authenticator(model);

                if (User_Authenticator && model.Name != null)
                {
                    // Se encontro el usuario.
                    HttpContext.Session.SetString("UserName", model.Name);
                    HttpContext.Session.SetString("IsAuthenticated", "true");
                    return RedirectToAction("Main");
                }
                else
                {
                    // Usuario no existe.
                    ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos.");
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Dejamos limpia la sesion
            return RedirectToAction("Login"); // Redirigimos a la pagina de inicio se sesion.
        }
    }
}
