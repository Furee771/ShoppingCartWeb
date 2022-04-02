using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;

namespace ShoppingCartWeb.Areas.Admin.Controllers
{
    
    [Area("Admin")]
     public class ProductController : Controller
     {
         private IUnitOfWork _unitfWork;
         private IWebHostEnvironment _hostEnvironment;
         public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment){
             _unitfWork = unitOfWork;
             _hostEnvironment = hostEnvironment;
         }
         #region APICALL

         public IActionResult AllProducts()
         {
             var products = _unitfWork.Product.GetAll(includeProperties: "Category");
             return Json(new{data=products});
             
         }
         #endregion
         public IActionResult Index()
         {
             // ProductVM productVM = new ProductVM();
             // ProductVM.products = _unitOfWork.Product.GetAll();
             return View();
         }
     }
}