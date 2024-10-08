﻿using BookingServices.Data;
using BookingServices.Models;
using BookingServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingServices.Controllers
{
    public class ProviderRegisterController : Controller
    {
        ApplicationDbContext _context;
        ErrorViewModel errorViewModel = new ErrorViewModel();

        public ProviderRegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var Providers = _context.ProviderRegisters.ToList();

            return View(Providers);
        }

        public IActionResult Create()
        {
            return View(new ProviderRegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProviderRegisterViewModel providerVM)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    ProviderRegister providerRegister = new ProviderRegister
                    {
                        ProviderName = providerVM.ProviderName,
                        ProviderPhoneNumber = providerVM.ProviderPhoneNumber,
                        ProviderEmail = providerVM.ProviderEmail,
                        ServiceDetails = providerVM.ServiceDetails,
                    };
                    _context.ProviderRegisters.Add(providerRegister);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Your data has been sent successfully!";

                    return RedirectToAction("Create");
                }

                return View(providerVM);
            }
            catch (Exception ex) 
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";

                return View(providerVM);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task <IActionResult> Delete(int id) 
        {
            if(id == 0)
            {
                errorViewModel.Message = $"An error occurred!";
                errorViewModel.Controller = "ProviderRegister";
                errorViewModel.Action = nameof(Index);

                return View("Error", errorViewModel);
            }

            ProviderRegister providerRegister =  await _context.ProviderRegisters.FindAsync(id);
            if(providerRegister == null)
            {
                errorViewModel.Message = $"Not Found!";
                errorViewModel.Controller = "ProviderRegister";
                errorViewModel.Action = nameof(Index);

                return View("Error", errorViewModel);
            }

            _context.ProviderRegisters.Remove(providerRegister);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
