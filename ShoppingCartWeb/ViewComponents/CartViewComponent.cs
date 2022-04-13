using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repositories;
using System.Security.Claims;

namespace ShoppingCartWeb.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIndentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIndentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claims != null)
            {
                if(HttpContext.Session.GetInt32("SessionCart") != null)
                {
                    return View(HttpContext.Session.GetInt32("SessionCart"));
                }
                else
                {
                    HttpContext.Session.SetInt32("SessionCart", _unitOfWork
                        .Cart.GetAll(x => x.ApplicationUserId == claims.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32("SessionCart"));
                }
            }
            else
            {
                HttpContext.Session.Clear(); 
                return View(0);
            }
        }
    }
}
