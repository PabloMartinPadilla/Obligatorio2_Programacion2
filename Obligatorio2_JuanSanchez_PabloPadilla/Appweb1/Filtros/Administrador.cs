using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace AppWeb.Filtros
{
    public class Admin : Attribute, IAuthorizationFilter
    {
        //Tomando el dato "tipo" se redirecciona al usuario si no es del tipo Administrador
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string rol = context.HttpContext.Session.GetString("tipo");
            if (string.IsNullOrEmpty(rol))
            {
                context.Result = new RedirectResult("/Login/Index");
                return;
            }
            if (rol != "Administrador")
            {
                context.Result = new RedirectResult("/Miembro/Index");
                return;
            }
        }
    }

}

