using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartWeb.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
