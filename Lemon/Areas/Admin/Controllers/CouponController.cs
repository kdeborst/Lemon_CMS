using Lemon.Data;
using Lemon.Models;
using Lemon.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lemon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class CouponController : Controller
    {
        //(LOCAL) PROPERTIES
        private readonly ApplicationDbContext _database;


        //CONSTRUCTOR
        public CouponController(ApplicationDbContext database)
        {
            _database = database;
        }


        //LOAD VIEW: INDEX - COUPON OVERVIEW
        public async Task<IActionResult> Index()
        {
            return View(await _database.Coupon.ToListAsync());
        }


        //LOAD VIEW: CREATE COUPON
        public IActionResult Create()
        {
            return View();
        }


        //ADD: COUPON TO DB
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCoupon(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    coupons.Picture = p1;
                }

                _database.Coupon.Add(coupons);
                await _database.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(coupons);
        }


        //LOAD VIEW: COUPON DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _database.Coupon.FirstOrDefaultAsync(m => m.Id == id);

            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }


        //LOAD VIEW: CHANGE COUPON
        public async Task<IActionResult> Change(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _database.Coupon.SingleOrDefaultAsync(m => m.Id == id);

            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }


        //CHANGE: COUPON IN DB
        [HttpPost, ActionName("Change")]
        public async Task<IActionResult> ChangeCoupon(Coupon coupons)
        {
            if (coupons.Id == 0)
            {
                return NotFound();
            }

            var couponFromDb = await _database.Coupon.Where(c => c.Id == coupons.Id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    couponFromDb.Picture = p1;
                }

                couponFromDb.Name = coupons.Name;
                couponFromDb.CouponType = coupons.CouponType;
                couponFromDb.Discount = coupons.Discount;
                couponFromDb.MinimumAmount = coupons.MinimumAmount;
                couponFromDb.IsActive = coupons.IsActive;

                await _database.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(coupons);
        }


        //LOAD VIEW: REMOVE COUPON
        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _database.Coupon.SingleOrDefaultAsync(m => m.Id == id);

            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }


        //REMOVE: COUPON FROM DB
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRemove(int id)
        {
            var coupons = await _database.Coupon.SingleOrDefaultAsync(m => m.Id == id);
            _database.Coupon.Remove(coupons);
            await _database.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}