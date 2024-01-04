using Dominio;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Filtros;

namespace Appweb1.Controllers
{
    public class RegistroController : Controller
    {
        private Sistema _sistema = Sistema.Instancia;

        //Controlador para la view Registro/Index
        public IActionResult Index()
        {
            return View(new Miembro());
        }

        //Controlador para la view Registro/Index donde el usuario ingresa los datos con metodo Post y de ser correcto se crea un nuevo Miembo y se lo redirige hacia el Login
        [HttpPost]
        public IActionResult Index(Miembro miembro)
        {
            try
            {
                _sistema.AgregarMiembro(miembro);
                return Redirect("/Login/Index");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return View(miembro);
        }

    }
}
