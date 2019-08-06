using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lemon.Models;
using Lemon.Models.ViewModels;
using Lemon.Data;
using Microsoft.EntityFrameworkCore;

namespace Lemon.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _database;


        //CONSTRUCTOR
        public HomeController(ApplicationDbContext database)
        {
            _database = database;
        }


        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _database.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _database.Category.ToListAsync(),
                Coupon = await _database.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };

            return View(IndexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
