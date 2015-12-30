using System.Web.Mvc;

namespace AfricanMovies.Web.UI.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Err/

        public ActionResult Index()
        {
            return View("Error");
        }

    }
}
