using System.Web.Mvc;

namespace AfricanMovies.Web.UI.Controllers
{
    public class AboutController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return RedirectToAction("Index");
        }
    }
}
