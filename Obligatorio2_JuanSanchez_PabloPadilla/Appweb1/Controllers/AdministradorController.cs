using AppWeb.Filtros;
using Dominio;
using Microsoft.AspNetCore.Mvc;

namespace Appweb1.Controllers
{
    [Admin]
    public class AdministradorController : Controller
    {
        private Sistema _sistema = Sistema.Instancia;

        // Controlador para la pagina principal (Administrador/Index) para el usuario Administrador
        public IActionResult Index()
        {
            try
            {
                ViewBag.Miembros = _sistema.ListarMiembrosOrdenados();

            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return View();
        }

        // Controlador para que el usuario Administrador pueda bloquear Miembros que no se encuentren bloqueados
        public IActionResult BloquearMiembro(string email)
        {
            try
            {
                _sistema.BloquearMiembro(email);
            }
            catch (Exception e) { ViewBag.Error = e.Message; }

            return RedirectToAction("Index");
        }

        // Controlador para la view Administrador/ListarPosts donde se listan todos los posts creados
        public IActionResult ListarPosts()
        {
            try
            {
                ViewBag.Posts = _sistema.ListarPosts();

            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return View();
        }

        // Controlador para que el usuario Administrador pueda censurar Posts que no se encuentren censurados
        public IActionResult CensurarPost(int id)
        {
            try
            {
                _sistema.CensurarPost(id);
            }
            catch (Exception e) { ViewBag.Error = e.Message; }

            return RedirectToAction("ListarPosts");
        }
    }
}

