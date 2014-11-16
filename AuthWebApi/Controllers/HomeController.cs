using AuthWebApi.Models.Account;
using System.Web.Mvc;

namespace AuthWebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExtAuthRequest()
        {
            return View();
        }
    }
}
