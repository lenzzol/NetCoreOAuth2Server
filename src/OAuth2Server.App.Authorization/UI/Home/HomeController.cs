using Microsoft.AspNetCore.Mvc;

namespace OAuth2Server.App.Authorization.UI.Home
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}