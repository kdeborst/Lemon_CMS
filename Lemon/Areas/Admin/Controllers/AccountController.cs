using Lemon.Data;
using Lemon.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lemon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class AccountController : Controller
    {
        //(LOCAL) PROPERTIES
        private readonly ApplicationDbContext _database;


        //CONSTRUCTOR
        public AccountController(ApplicationDbContext database)
        {
            _database = database;
        }

        //LOAD VIEW: INDEX - OVERVIEW OF ALL ACCOUNTS != LOGGED-IN USER
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _database.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync());
        }


        //ACTION: LOCK EMPLOYEE ACCOUNTS
        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _database.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);
            await _database.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //ACTION: UNLOCK EMPLOYEE ACCOUNTS
        public async Task<IActionResult> Unlock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _database.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now;
            await _database.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}