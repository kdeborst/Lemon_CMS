using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lemon.Data;
using Lemon.Models;
using Lemon.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lemon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _database;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext database)
        {
            _database = database;
        }

        //RETRIEVE ALL SUBCATEGORIES
        public async Task<IActionResult> Index()
        {
            var subCategories = await _database.SubCategory.Include(s=>s.Category).ToListAsync();
            return View(subCategories);
        }

        //LOAD CREATE SUBCATEGORY VIEW
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _database.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _database.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //ADD SUBCATEGORY TO DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkExistenceSubCategory = _database.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                
                if(checkExistenceSubCategory.Count() > 0)
                {
                    StatusMessage = "Error: " + checkExistenceSubCategory.First().Category.Name + " bestaat al! Gebruik een andere naam voor je subcategorie.";
                }
                else
                {
                    _database.SubCategory.Add(model.SubCategory);
                    await _database.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _database.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _database.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };

            return View(modelVM);
        }

        //RETRIEVE LIST OF EXISTING SUBCATEGORIES FOR OVERVIEW
        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await (from subCategory in _database.SubCategory where subCategory.CategoryId == id select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }
    }
}