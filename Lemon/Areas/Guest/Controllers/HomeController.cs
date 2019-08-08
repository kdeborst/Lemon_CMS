using Lemon.Data;
using Lemon.Models;
using Lemon.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lemon.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        //(LOCAL) PROPERTIES
        private readonly ApplicationDbContext _database;


        //CONSTRUCTOR
        public HomeController(ApplicationDbContext database)
        {
            _database = database;
        }


        //LOAD VIEW: LANDINGPAGE
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


        //LOAD VIEW: PRIVACY STATEMENT
        public IActionResult Privacy()
        {
            return View();
        }


        //LOAD VIEW: ERROR
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
