﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Utility;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace ShoppingCartWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        #region APICALL
        public IActionResult AllOrders(string status)
        {
            IEnumerable<OrderHeader> orderHeader;
            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                orderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeader = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == claims.Value);
            }

            switch (status)
            {
                case "pending":
                    orderHeader = orderHeader.Where(x => x.PaymentStatus == PaymentStatus.StatusPending);
                    break;
                case "approved":
                    orderHeader = orderHeader.Where(x => x.PaymentStatus == PaymentStatus.StatusApproved);
                    break;
                case "underprocess":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == OrderStatus.StatusInProcess);
                    break;
                case "shipped":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == OrderStatus.StatusShpped);
                    break;

                    default:
                    break;
            }
            return Json(new {data =orderHeader});
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult OrderDetails(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product")
            };
            return View(orderVM);
        }
        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        [HttpPost]
        public IActionResult OrderDetails(OrderVM vm)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == vm.OrderHeader.Id);
            orderHeader.Name = vm.OrderHeader.Name;
            orderHeader.Phone = vm.OrderHeader.Phone;  
            orderHeader.Address = vm.OrderHeader.Address;  
            orderHeader.City = vm.OrderHeader.City;
            orderHeader.State = vm.OrderHeader.State;
            orderHeader.PostalCode = vm.OrderHeader.PostalCode;
            if(vm.OrderHeader.Carrier != null)
            {
                orderHeader.Carrier = vm.OrderHeader.Carrier;  
            }
            if (vm.OrderHeader.TrackingNumber != null)
            {
                orderHeader.TrackingNumber = vm.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Info Update";
            return RedirectToAction("OrderDetails", "Order", new { id = vm.OrderHeader.Id });
        }

        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public ActionResult InProcess(OrderVM vm)
        {
            _unitOfWork.OrderHeader.UpdateStatus(vm.OrderHeader.Id, OrderStatus.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated - Inproces";
            return RedirectToAction("OrderDetails", "Order", new { id = vm.OrderHeader.Id });
        }
        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public IActionResult Shipped(OrderVM vm)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == vm.OrderHeader.Id);
            orderHeader.Carrier = vm.OrderHeader.Carrier;
            orderHeader.TrackingNumber =vm.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = OrderStatus.StatusShpped;
            orderHeader.DateOfShopping = DateTime.Now;

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated-Shipped";
            return RedirectToAction("OrderdDetails", "Order", new {id = vm.OrderHeader.Id});
        }
        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public IActionResult CancelOrder(OrderVM vm)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == vm.OrderHeader.Id);
            if(orderHeader.PaymentStatus == PaymentStatus.StatusApproved)
            {
                var refund = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund Refund = service.Create(refund);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, OrderStatus.StatusCancelled, OrderStatus.StatusRefund);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, OrderStatus.StatusCancelled, OrderStatus.StatusRefund);
            }
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled";
            return RedirectToAction("OrderDetails", "Order", new  { id = vm.OrderHeader.Id });
        }

        public IActionResult PayNow(OrderVM vm)
        {
            var OrderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == vm.OrderHeader.Id,includeProperties: "ApplicationUser");
            var OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == vm.OrderHeader.Id, includeProperties: "Product");
            var domain = "https://localhost:7047/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/ordersuccess?id={vm.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/Index",
            };

            foreach (var item in OrderDetail)
            {
                var lineItemsOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "INR",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(lineItemsOptions);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.PaymentStatus(vm.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            return RedirectToAction("Index", "Home");
        }
    }
}