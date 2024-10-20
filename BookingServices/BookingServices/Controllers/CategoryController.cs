﻿using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        ApplicationDbContext context;
        ErrorViewModel errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };

        public CategoryController([FromServices] ApplicationDbContext _context)
        {
            context = _context;
        }

        public ActionResult Index()
        {
            return View(context.Categories.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                context.Categories.Add(category);
                try
                {
                    context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    errorViewModel = new ErrorViewModel { Message = e.Message, Controller = "Category", Action = "Index" };
                    return View("Error", errorViewModel);
                }
            }
            return View(category);
        }

        public ActionResult Edit(int id)
        {
            var category = context.Categories.Find(id);
            if (category == null)
            {
                errorViewModel = new ErrorViewModel { Message = "No Category Valid With This ID", Controller = "Category", Action = "Index" };
                return View("Error", errorViewModel);
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                context.Categories.Update(category);
                try
                {
                    context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    errorViewModel = new ErrorViewModel { Message = e.Message, Controller = "Category", Action = "Index" };
                    return View("Error", errorViewModel);
                }
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                errorViewModel = new ErrorViewModel { Message = "Need ID For Category", Controller = "Category", Action = "Index" };
                return View("Error", errorViewModel);
            }
            var category = context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                errorViewModel = new ErrorViewModel { Message = "Category Not Exist", Controller = "Category", Action = "Index" };
                return View("Error", errorViewModel);
            }

            return View(category);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var category = context.Categories.Find(id);
            if (category == null)
            {
                errorViewModel = new ErrorViewModel { Message = "Category Not Exist", Controller = "Category", Action = "Index" };
                return View("Error", errorViewModel);
            }
            var services = context.Services;
            foreach (var service in services)
            {
                if (service.CategoryId == category.CategoryId)
                {
                    service.CategoryId = null;
                    context.SaveChanges();
                }
            }
            context.Categories.Remove(category);
            try
            {
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = "", Controller = "Category", Action = "Index" };
                return View("Error", errorViewModel);
            }
        }
    }
}
