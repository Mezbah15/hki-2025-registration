using hki_2025_registration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hki_2025_registration.Controllers
{
    public class InventoryController : Controller
    {

        public ActionResult StallRegistration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StallRegistration(ShopKeeper model)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}
