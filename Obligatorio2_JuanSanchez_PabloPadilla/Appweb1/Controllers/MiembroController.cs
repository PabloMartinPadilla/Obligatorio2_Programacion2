using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Hosting;
using AppWeb.Filtros;

namespace Appweb1.Controllers
{
    [Member]
    public class MiembroController : Controller
    {
        private Sistema _sistema = Sistema.Instancia;

        //Controlador para la view Miembro/Index donde se listan todos los post permitidos del usuario ingresado
        public IActionResult Index()
        {
            try
            {
                ViewBag.Posts = _sistema.BuscarPostPorEmail(HttpContext.Session.GetString("email"));
                string email = HttpContext.Session.GetString("email");
                Miembro autor = _sistema.BuscarMiembroPorEmail(email);
                ViewBag.Bloqueado = autor.Bloqueado;
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return View();
        }

        //Controlador para la view Miembro/Amigos donde se listan los usuarios que no compartan invitación
        public IActionResult Amigos()
        {
            try
            {
                ViewBag.PosiblesAmigos = _sistema.BuscarPosiblesAmigos(HttpContext.Session.GetString("email"));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return View(new Invitacion());
        }

        //Controlador para la view Miembro/Amigos donde se toma el dato del usuario a mandar invitación y se crea la misma
        [HttpPost]
        public IActionResult Amigos(string email)
        {
            try
            {
                Miembro miembro = _sistema.BuscarMiembroPorEmail(HttpContext.Session.GetString("email"));
                Miembro nuevoAmigo = _sistema.BuscarMiembroPorEmail(email);
                _sistema.CrearInvitacionAmistad(miembro, nuevoAmigo);
                return RedirectToAction("amigos");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            ViewBag.PosiblesAmigos = _sistema.BuscarPosiblesAmigos(HttpContext.Session.GetString("email"));
            return View();
        }

        //Controlador para la view Miembro/Invitaciones donde se listan las invitaciones pendientes del miembro
        public IActionResult Invitaciones()
        {
            try
            {
                ViewBag.InvitacionesPendientes = _sistema.BuscarInvitacionesPendientes(HttpContext.Session.GetString("email"));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return View();
        }

        //Controlador para que el Miembro pueda aceptar la invitación pendiente seleccionada
        public IActionResult AceptarInvitacion(int id)
        {
            try
            {
                _sistema.AceptarInvitacion(id);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return RedirectToAction("Invitaciones");
        }

        //Controlador para que el Miembro pueda rechazar la invitación pendiente seleccionada
        public IActionResult RechazarInvitacion(int id)
        {
            try
            {
                _sistema.RechazarInvitacion(id);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return RedirectToAction("Invitaciones");
        }

        //Controlador para la view Miembro/CrearPost donde el miembro puede crear un nuevo post si no se encuentra bloqueado
        public IActionResult CrearPost()
        {
            Miembro miembro = _sistema.BuscarMiembroPorEmail(HttpContext.Session.GetString("email"));
            ViewBag.Bloqueado = miembro.Bloqueado;
            ViewBag.Privacidades = _sistema.TiposPrivacidad();
            return View(new Post());

            //Controlador donde se toman los datos ingresados (por Post) por el miembro, se crea el post si cumple con las validación correspondientes y se lo redirige hacía la pagina principal
        }
        [HttpPost]
        public IActionResult CrearPost(Post post)
        {
            Miembro miembro = _sistema.BuscarMiembroPorEmail(HttpContext.Session.GetString("email"));
            if (miembro.Bloqueado == false)
            {
                try
                {
                    string email = HttpContext.Session.GetString("email");
                    Miembro autor = _sistema.BuscarMiembroPorEmail(email);
                    post.Autor = autor;
                    post.Fecha = DateTime.Now;
                    post.Censurado = false;
                    _sistema.AgregarPost(post);
                    ViewBag.Posts = _sistema.BuscarPostPorEmail(HttpContext.Session.GetString("email"));
                    return RedirectToAction("index");
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                }
            }
            else
            {
                ViewBag.Error = "Este usuario se encuentra bloqueado por lo tanto no puede crear Posts";
            }
            ViewBag.Bloqueado = miembro.Bloqueado;
            ViewBag.Privacidades = _sistema.TiposPrivacidad();
            return View(post);
        }

        //Controlador para la view Miembro/CrearComentario donde el miembro puede crear un nuevo comentario si no se encuentra bloqueado
        public IActionResult CrearComentario(int idPost)
        {
            ViewBag.idPost = idPost;
            return View(new Comentario());
        }

        //Controlador donde se toman los datos ingresados (por Post) por el miembro, se crea el comentario asociado a su Post si cumple con las validación correspondientes y se lo redirige hacía la pagina principal
        [HttpPost]
        public IActionResult CrearComentario(Comentario comentario, int idPost)
        {
            try
            {
                Post post = _sistema.BuscarPostPorId(idPost);
                Miembro miembro = _sistema.BuscarMiembroPorEmail(HttpContext.Session.GetString("email"));

                if (miembro.Bloqueado == false && post != null)
                {
                    try
                    {
                        comentario.Privacidad = post.Privacidad;
                        comentario.Post = post;
                        comentario.Fecha = DateTime.Now;
                        comentario.Autor = miembro;
                        _sistema.AgregarComentario(comentario, post);
                        return RedirectToAction("index");
                    }
                    catch (Exception e)
                    {
                        ViewBag.Error = e.Message;
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            ViewBag.idPost = idPost;
            return View(comentario);
        }

        //Controlador para que el Miembro pueda reaccional al Post con un like si no lo había hecho previamente
        public IActionResult DarLike(int id)
        {
            Miembro miembro = _sistema.BuscarMiembroPorEmail(HttpContext.Session.GetString("email"));
            _sistema.AgregarLikePorId(id, miembro);
            ViewBag.Posts = _sistema.BuscarPostPorEmail(HttpContext.Session.GetString("email"));
            string email = HttpContext.Session.GetString("email");
            Miembro autor = _sistema.BuscarMiembroPorEmail(email);
            ViewBag.Bloqueado = autor.Bloqueado;
            return View("Index");
        }

        //Controlador para que el Miembro pueda reaccional al Post con un dislike si no lo había hecho previamente
        public IActionResult DarDisLike(int id)
        {
            Miembro miembro = _sistema.BuscarMiembroPorEmail(HttpContext.Session.GetString("email"));
            _sistema.AgregarDisLikePorId(id, miembro);
            ViewBag.Posts = _sistema.BuscarPostPorEmail(HttpContext.Session.GetString("email"));
            string email = HttpContext.Session.GetString("email");
            Miembro autor = _sistema.BuscarMiembroPorEmail(email);
            ViewBag.Bloqueado = autor.Bloqueado;
            return View("Index");
        }

        //Controlador para la view Miembro/FiltrarPublicaciones
        public IActionResult FiltrarPublicaciones()
        {
            return View();
        }

        //Controlador para la view Miembro/FiltrarPublicaciones donde el miembro proporciona datos por metodo Post y se le listan las Publicaciones que cumplan con las condiciones 
        [HttpPost]
        public IActionResult FiltrarPublicaciones(string texto, int valorAceptacion)
        {
            ViewBag.PublicacionesFiltradas = _sistema.FiltrarPublicaciones(texto, valorAceptacion);
            return View();
        }
    }
}