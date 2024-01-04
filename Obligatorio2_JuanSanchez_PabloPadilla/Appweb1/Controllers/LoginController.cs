using Dominio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Filtros;

namespace Appweb1.Controllers
{
    public class LoginController : Controller
    {
        private Sistema _sistema = Sistema.Instancia;

        //Controlador para la view Login/Index con datos vacios
        public IActionResult Index()
        {
            return View();
        }

        //Controlador para tomar los datos de la view Login/Index por Post y si se ingresa correctamente retorna al usuario a su respectiva pagina principal
        [HttpPost]
        public IActionResult Index(string email, string contrasena)
        {
            try
            {
                Usuario usuario = _sistema.BuscarUsuarioPorEmail(email);
                if (usuario == null || contrasena != usuario.Contrasena)
                {
                    ViewBag.Error = "Usuario y/o contraseña incorrecta";
                    return View("Index");
                }
                HttpContext.Session.SetString("email", email);
                HttpContext.Session.SetString("tipo", usuario.GetType().Name);

                if (HttpContext.Session.GetString("tipo") == "Administrador")
                {
                    return Redirect("/Administrador/Index");
                }
                else
                {
                    return Redirect("/Miembro/Index");
                }
            }
            catch
            {
                return View("Index");
            }
        }

        //Controlador para eliminar los datos al ingresar y retornar al usuario hacía el Login
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
