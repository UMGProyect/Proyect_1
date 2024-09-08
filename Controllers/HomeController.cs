using Microsoft.AspNetCore.Mvc;
using Proyect_1.ContextBD;
using Proyect_1.Models;
using Proyect_1.Services;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Proyect_1.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;
        //Instancia de sesión.
        private readonly Sesion iniciar_sesion = new();
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

           
                // Obtener el número de intentos fallidos desde la sesión
                int loginAttempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;

                //Revisar esto. El captcha se valida para ver si entra en true o false desde https
                //Se maneja otra validacion de captcha, se almacen en httpcontext el input del captcha antes de compararse.
                string CaptchaValidation = HttpContext.Session.GetString("Captcha")?? "false";
            var captchaInput = Request.Form["Captcha"].ToString().Trim('{', '}');

            var captchaText = HttpContext.Session.GetString("CaptchaText");
                
                

                loginAttempts++;
                HttpContext.Session.SetInt32("LoginAttempts", loginAttempts);
                // Mostrar CAPTCHA si se ha alcanzado el límite de intentos
                if (loginAttempts >= 7 && CaptchaValidation=="false")
                {
                    
                    model.Captcha = true;
                        HttpContext.Session.SetString("Captcha", "true");
                        ViewData["ShowCaptcha"] = model.Captcha;

                        if (captchaText != captchaInput)
                        {
                            ModelState.AddModelError("", "El CAPTCHA es incorrecto.");
                            return View(model);
                        }
                    else if (  loginAttempts > 7)
                    {
                        HttpContext.Session.SetInt32("LoginAttempts", 0);
                        model.Captcha = false;
                        HttpContext.Session.SetString("Captcha", "false");
                        ViewData["ShowCaptcha"] = model.Captcha;
                    }
                }
                
                else if (captchaText != captchaInput && loginAttempts > 7 )
                {
                    model.Captcha = true;
                    ViewData["ShowCaptcha"] = model.Captcha;
                    ModelState.AddModelError("", "El CAPTCHA es incorrecto.");
                    return View(model);
                }else if (loginAttempts > 7)
                {
                    HttpContext.Session.SetInt32("LoginAttempts", 0);
                    model.Captcha = false;
                    HttpContext.Session.SetString("Captcha", "false");
                    ViewData["ShowCaptcha"] = model.Captcha;
                }

            // Validaciones de entradas
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError("", "El nombre de usuario y la contraseña no pueden estar vacíos.");
                    return View(model);
                }

                if (model.Name.Contains(" ") || model.Password.Contains(" "))
                {
                    ModelState.AddModelError("", "El nombre de usuario y la contraseña no deben contener espacios.");
                    return View(model);
                }

                if (model.Name.Length < 3 || model.Name.Length > 10)
                {
                    ModelState.AddModelError("", "El nombre de usuario debe tener entre 3 y 10 caracteres.");
                    return View(model);
                }

                if (model.Password.Length < 5 || model.Password.Length > 15)
                {
                    ModelState.AddModelError("", "La contraseña debe tener entre 5 y 15 caracteres.");
                    return View(model);
                }

                bool User_Authenticator;
                try
                {
                    User_Authenticator = iniciar_sesion.Authenticator(model,"0");

                }
                catch (Exception ex)
                {
                HttpContext.Session.SetString("ErrorId", ex.Message);
                
                    var errorReport = new ErrorReport
                    {
                        ErrorDetectado = "¡Se encontró un error!"
                    };
                    return View("UserReportsBugs", errorReport);
                }

                if (User_Authenticator && model.Name != null)
                {
                    // Autenticación exitosa
                    HttpContext.Session.SetString("UserName", model.Name);
                    HttpContext.Session.SetString("IsAuthenticated", "true");
                   
                    HttpContext.Session.Remove("LoginAttempts"); // Restablecer el contador de intentos
                    return RedirectToAction("Main");
                }
                else
                {
                    
                    // Mostrar error si se han alcanzado 7 intentos fallidos
                    if (loginAttempts >= 7)
                    {
                        ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos. Resuelva el CAPTCHA para continuar.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos.");
                    }

                    return View(model);
                }
            
            return View(model);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Dejamos limpia la sesion
            return RedirectToAction("Login"); // Redirigimos a la pagina de inicio se sesion.
        }

        // Manejo de seguridad

        [HttpGet]
        public  async Task<IActionResult> CaptchaImage()
        {
            string captchaText = GenerateCaptchaText();
            HttpContext.Session.SetString("CaptchaText", captchaText);

            using (var bitmap = new Bitmap(200, 50))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                using (var font = new Font("Arial", 24, FontStyle.Bold))
                {
                    graphics.DrawString(captchaText, font, Brushes.Black, 10, 10);
                }

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();

                    // Usar WriteAsync en lugar de Write
                     return File(imageBytes, "image/png");
                }
            }
        }
        
        private string GenerateCaptchaText()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }

        //Manejo de reportes - Sentry


        public IActionResult SystemReport()
        {
            return View();
        }


        public IActionResult UserReportsBugs()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReportError(ErrorReport model)
        {

            // Obtener el mismo identificador único almacenado previamente
            string errorId = HttpContext.Session.GetString("ErrorId");

            // Configurar el scope para agregar el ErrorId

            // Capturar el mensaje asociado al reporte de error
            SentrySdk.CaptureMessage($"Error reportado por el usuario: {model.Description}  Horror encontrado: {errorId}");

            // Redirigir o mostrar un mensaje de confirmación
            return RedirectToAction("Login");


            // Si el modelo no es valido, vuelve a mostrar el formulario
            //return View("UserReportsBugs", model);
        }
    }
}
