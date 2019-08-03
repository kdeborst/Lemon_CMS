using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lemon.Data;
using Lemon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lemon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _database;

        public CategoryController(ApplicationDbContext database)
        {
            _database = database;
        }

        //RETRIEVE ALL CATEGORIES
        public async Task<IActionResult> Index()
        {
            return View(await _database.Category.ToListAsync());
        }

        //LOAD CREATE CATEGORY VIEW
        public IActionResult Create()
        {
            return View();
        }

        //ADD CATEGORY TO DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _database.Category.Add(category);
                await _database.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //LOAD CATEGORY DETAILS VIEW
        public async Task<IActionResult> Details(int ? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _database.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //LOAD CREATE CATEGORY VIEW
        public async Task<IActionResult> Change(int ? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var category = await _database.Category.FindAsync(id);

            if(category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //CHANGE CATEGORY IN DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(Category category)
        {
            if(ModelState.IsValid)
            {
                _database.Update(category);
                await _database.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //LOAD VIEW & PASS REMOVE CATEGORY OPTION
        public async Task<IActionResult> Remove(int ? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _database.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //REMOVE CATEGORY IN DB
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRemove(int id)
        {
            var category = await _database.Category.FindAsync(id);

            if(category == null)
            {
                return View(); 
            }

            _database.Category.Remove(category);
            await _database.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}