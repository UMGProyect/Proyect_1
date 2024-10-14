using Microsoft.AspNetCore.Mvc;
using Proyect_1.ContextBD;
using Proyect_1.Models;
using Proyect_1.Services;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Proyect_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey; //permitir null para _secretKey
        private readonly BlobService _blobService;
        private readonly BD_User _userService;

        private readonly ILogger<HomeController> _logger;
        //Instancia de sesión.
        private readonly Sesion iniciar_sesion = new();
        private readonly ReportService _reportService;

        // constructor con inyeccion del servicio
        public HomeController(ILogger<HomeController> logger, ReportService reportService, BlobService blobService, BD_User userService)

        {

            _logger = logger;
            _reportService = reportService;
            _httpClient = new HttpClient(); //Inicializa HttpCliente
            _secretKey = "mi_secreto"; //proporciona un valor para la clave secreta
            _blobService = blobService;
            _userService = userService;

        }


        //***************************

        //Publicar algo
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CrearPublicacion(string title, string content, IFormFile mediaFile)
        {
            string userName = HttpContext.Session.GetString("UserName");

            string mediaUrl = null;

            if (mediaFile != null && mediaFile.Length > 0)
            {
                mediaUrl = await _blobService.UploadFileAsync(mediaFile);
                if (mediaUrl == null)
                {
                    ModelState.AddModelError("file", "Error al cargar el archivo.");
                    return View(); // Retorna la vista con error
                }
            }

            _userService.CreatePost(userName, title, content, mediaUrl); // Pasamos title, content y mediaUrl
            return RedirectToAction("Perfil");
        }




        //Metodo para subir una imagen al servidor de archivos. 
        [HttpPost]
        public async Task<IActionResult> EditarPerfil(IFormFile profilePictureFile, IFormFile bannerFile)
        {
            // Verifica si se ha subido una nueva foto de perfil
            if (profilePictureFile != null && profilePictureFile.Length > 0)
            {
                string profilePictureUrl = await _blobService.UploadFileAsync(profilePictureFile);

                // Verifica si la URL de la foto de perfil es null
                if (profilePictureUrl == null)
                {
                    ModelState.AddModelError("profilePictureFile", "Error al cargar la foto de perfil.");
                    return View(); // Retorna la vista con error
                }

                // Actualiza la foto de perfil
                _userService.UpdateProfilePicture(HttpContext.Session.GetString("UserName"), profilePictureUrl);
            }

            // Verifica si se ha subido un nuevo banner
            if (bannerFile != null && bannerFile.Length > 0)
            {
                string bannerUrl = await _blobService.UploadFileAsync(bannerFile);

                // Verifica si la URL del banner es null
                if (bannerUrl == null)
                {
                    ModelState.AddModelError("bannerFile", "Error al cargar el banner.");
                    return View(); // Retorna la vista con error
                }

                // Actualiza la URL del banner
                _userService.UpdateBannerUrl(HttpContext.Session.GetString("UserName"), bannerUrl);
            }
            else
            {
                ModelState.AddModelError("bannerFile", "Por favor selecciona un archivo para el banner.");
            }

            return RedirectToAction("Perfil"); // Redirige a la vista del perfil
        }



        //***************************




        // Aquí agregamos el nuevo método ConsultarDatos
        [HttpGet]
        public async Task<IActionResult> ConsultarDatos()
        {
            // Llamar al servicio para obtener los datos simulados
            var datos = await _reportService.ConsultarDatos();
            // Retornar los datos en formato JSON
            return Json(datos);
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

        //Registro
        public IActionResult Registrar()
        {

            return View();
        }

        public IActionResult Registro(User model)
        {
            // Verificar si el modelo es válido antes de proceder
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Registrar");
            }

            // Lógica para registrar el usuario
            bool success = iniciar_sesion.RegisterUser(model);
            if (success)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "El nombre de usuario ya existe.");
            }

            return RedirectToAction("Registrar");
        }



        //PERFIL
        public IActionResult PerfilUser()
        {

            return RedirectToAction("Perfil");
        }

        public IActionResult Perfil()
        {
            // Obtener el nombre de usuario de la sesión
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return NotFound(); // Manejo de error si no se encuentra el nombre de usuario
            }

            // Obtener el perfil del usuario utilizando el servicio
            var viewModel = _userService.GetUserProfile(userName);

            if (viewModel == null)
            {
                return NotFound(); // Manejo de error si no se encuentra el perfil
            }
            return View(viewModel); // Devuelve la vista con el modelo del perfil de usuario
        }

        public IActionResult MenuPrincipal()
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
        public async Task<IActionResult> Login(User model)
        {
            int loginAttempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;

            // Si el reCAPTCHA está activo, valida el token
            if (loginAttempts >= 5)
            {
                string token = model.RecaptchaToken; // model.RecaptchaToken
                var recaptchaResponse = await ValidateRecaptcha(token);
                if (!recaptchaResponse)
                {
                    ModelState.AddModelError("", "Verificación de reCAPTCHA fallida. Inténtalo de nuevo.");
                    return View(model);
                }
                HttpContext.Session.Remove("LoginAttempts");
            }
            loginAttempts++;
            HttpContext.Session.SetInt32("LoginAttempts", loginAttempts);


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
                User_Authenticator = iniciar_sesion.Authenticator(model, "0");
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
                ViewBag.IsAuthenticated = HttpContext.Session.GetString("IsAuthenticated");
                HttpContext.Session.Remove("LoginAttempts"); // Restablecer el contador de intentos
                return RedirectToAction("Perfil");
            }
            else
            {
                ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos.");
                return View(model);
            }
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Dejamos limpia la sesion
            return RedirectToAction("Login"); // Redirigimos a la pagina de inicio se sesion.
        }

        // Manejo de seguridad

        public async Task<bool> ValidateRecaptcha(string token)
        {
            // Crear el contenido de la solicitud
            var postData = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("secret", "6Lepo1gqAAAAAJqO1RhlNUP5wE5cN0ZV3CAbNeOU"),
            new KeyValuePair<string, string>("response", token)
        });

            // Enviar la solicitud POST
            // var response = await _httpClient.PostAsync("https://recaptchaenterprise.googleapis.com/v1/projects/umgproyect-1726805979105/assessments?key=API_KEY", postData);

            if (token != null)
            {
                return true;
            }

            return false; // En caso de error, retorna false
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

        }

        //NUEVA ACCION PARA LA VISTA DE REPORTES
        public IActionResult Reportes()
        {
            //simulacion de datos relevantes para una red social. En una aplicacion real, estos datos se obtendrian de la BD 
            List<Reporte> listaReportes = new List<Reporte>
            {
                new Reporte { Id = 1, Nombre = "Reporte de Actividad de Usuarios ", Descripcion = "Informe detallado sobre la actividad de los usuarios", Fecha = DateTime.Now },
                new Reporte { Id = 2, Nombre = "Reporte de Contenido Moderado", Descripcion = "Resumen de contenido que ha sido revisado y moderado por el equipo de administracion", Fecha = DateTime.Now.AddDays(-7) },
                new Reporte { Id = 3, Nombre = "Reporte de Engagement", Descripcion = "Analisis de la interaccion de los usuarios con las publicaciones y funcionalidades de la red social ", Fecha = DateTime.Now.AddDays(-30) },
                new Reporte { Id = 4, Nombre = "Reporte de Nuevos Usuarios", Descripcion = "Cantidad de nuevos usuarios registrados en la plataforma", Fecha = DateTime.Now.AddDays(-15)},
                new Reporte { Id = 5, Nombre = "Reporte de contenido Popular", Descripcion = "Lista de las Publicaciones mas populares segun numero de 'me gusta', comentarios y compartidos.", Fecha = DateTime.Now },
                };

            //pasar la lista de reportes como modelo a la vista.
            return View(listaReportes);

        }

    }
}