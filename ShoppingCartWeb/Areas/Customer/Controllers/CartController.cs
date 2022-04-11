using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using System.Security.Claims;

namespace ShoppingCartWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            CartVM vm = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties : "Product"),
                OrderHeader = new OrderHeader()
            };
            foreach(var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }

            return View(vm);
        }

        public ActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            CartVM vm = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            vm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetT(x => x.Id == claims.Value);
            vm.OrderHeader.Name = vm.OrderHeader.ApplicationUser.Name;
            vm.OrderHeader.Phone = vm.OrderHeader.ApplicationUser.PhoneNumber;
            vm.OrderHeader.Address = vm.OrderHeader.ApplicationUser.Address;
            vm.OrderHeader.City = vm.OrderHeader.ApplicationUser.City;
            vm.OrderHeader.State = vm.OrderHeader.ApplicationUser.State;
            vm.OrderHeader.PostalCode = vm.OrderHeader.ApplicationUser.PinCode;

            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult Plus(int id)
        { 
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
           // _unitOfWork.Cart.IncrementCartItem(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            if (cart.Count <= 1)
            {
                _unitOfWork.Cart.Delete(cart);
            }
            else
            {
            //    _unitOfWork.Cart.DecrementCartItem(cart, 1);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            _unitOfWork.Cart.Delete(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
