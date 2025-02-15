using System.Security.Claims;
using hki_2025_registration.Models;
using hki_2025_registration.PaginationHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace hki_2025_registration.Controllers
{
    public class DataAccessPS1582Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public DataAccessPS1582Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> PartiCipant1318(int? pageIndex)
        {
            int pageSize = 50;

            var query = _context.Participants.AsQueryable();

            var paginatedList = await PaginatedList<Participant>.CreateAsync(query, pageIndex ?? 1, pageSize);

            return View(paginatedList);
        }
        
        public async Task<IActionResult> StallReg1578(int? pageIndex)
        {
            int pageSize = 80;

            var query = _context.ShopKeepers.AsQueryable();

            var paginatedList = await PaginatedList<ShopKeeper>.CreateAsync(query, pageIndex ?? 1, pageSize);

            return View(paginatedList);
        }

    }
}
