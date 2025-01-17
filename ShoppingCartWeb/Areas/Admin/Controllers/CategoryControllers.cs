﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;

namespace ShoppingCartWeb.Areas.Admin.Controllers
{
        [Area("Admin")] //админ
        [Authorize(Roles ="Admin")]
        public class CategoryController : Controller
        {
            private IUnitOfWork _unitOfWork;

            public CategoryController(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public IActionResult Index()
            {
                CategoryVM categoryVM = new CategoryVM();
                categoryVM.categories = _unitOfWork.Category.GetAll();
                return View(categoryVM);
            }
            //

            [HttpGet]
            public IActionResult CreateUpdate(int? id)
            {
                CategoryVM vm = new CategoryVM();
                if (id == null || id == 0)
                {
                    return View(vm);
                }
                else
                {
                    vm.Category = _unitOfWork.Category.GetT(x => x.Id == id);
                    if (vm.Category == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return View(vm);
                    }
                }
            }
            [HttpPost]
            [AutoValidateAntiforgeryToken]
            public IActionResult CreateUpdate(CategoryVM vm)
            {
                if (ModelState.IsValid)
                {
                    if (vm.Category.Id == 0)
                    {
                        _unitOfWork.Category.Add(vm.Category);
                        TempData["success"] = "Category Created Done!";
                    }
                    else
                    {
                        _unitOfWork.Category.Update(vm.Category);
                        TempData["success"] = "Category Created Done!";
                    }
                    _unitOfWork.Save();

                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }

            [HttpGet]
            public IActionResult Delete(int? id)
            {
                if(id == null || id == 0)
                {
                    return NotFound();
                }
                var category = _unitOfWork.Category.GetT(x => x.Id == id);
                if(category == null)
                {
                    return NotFound();
                }
                return View(category);
            }

            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public IActionResult DeleteData(int? id)
            {
                var category = _unitOfWork.Category.GetT(x => x.Id == id);
                if (category == null)
                {
                    return NotFound();
                }
                _unitOfWork.Category.Delete(category);
                _unitOfWork.Save();
                TempData["success"] = "Category Delete Done!";
                return RedirectToAction("Index");
            }
        }
 }
