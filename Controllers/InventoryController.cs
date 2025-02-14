using System.Text.RegularExpressions;
using hki_2025_registration.Models;
using hki_2025_registration.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace hki_2025_registration.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public InventoryController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public ActionResult StallRegistration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StallRegistration(ShopKeeper model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (!Regex.IsMatch(model.Contact, @"^(?:\+88|88)?(01[3-9]\d{8})$"))
                {
                    ModelState.AddModelError("Contact", "Invalid contact number format.");
                    return View(model);
                }

                _context.ShopKeepers.Add(model);
                await _context.SaveChangesAsync();

                TempData["Message"] = "ধন্যবাদ। শীঘ্রই আপনার সাথে যোগাযোগ করা হবে ইনশাআল্লাহ।\r\n";

                _logger.LogInformation("Stall Registration Form Submitted");

                return RedirectToAction("StallRegistration");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Stall Registration Form Submit");
                ModelState.AddModelError(string.Empty, "Something went wrong, Contact Administrator.");
                return View(model);
            }
        }

    }
}
