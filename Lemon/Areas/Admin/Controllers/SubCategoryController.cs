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


        //CONSTRUCTOR
        public SubCategoryController(ApplicationDbContext database)
        {
            _database = database;
        }


        //LOAD VIEW: INDEX - OVERVIEW EXISTING SUBCATEGORIES
        public async Task<IActionResult> Index()
        {
            var subCategories = await _database.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }


        //LOAD VIEW: CREATE SUBCATEGORY
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


        //CREATE: SUBCATEGORY IN DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkExistenceSubCategory = _database.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (checkExistenceSubCategory.Count() > 0)
                {
                    StatusMessage = "Error: de ingevoerde subcategorie bestaat al in " + checkExistenceSubCategory.First().Category.Name + "! Gebruik een andere naam.";
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


        //RETURN LIST: EXISTING SUBCATEGORIES
        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await (from subCategory in _database.SubCategory where subCategory.CategoryId == id select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        //LOAD VIEW: SUBCATEGORY DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                NotFound();
            }

            var subCategory = await _database.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                NotFound();
            }

            return View(subCategory);
        }


        //LOAD VIEW: CHANGE SUBCATEGORY
        public async Task<IActionResult> Change(int? id)
        {
            if (id == null)
            {
                NotFound();
            }

            var subCategory = await _database.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _database.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _database.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }


        //UPDATE: SUBCATEGORY IN DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkExistenceSubCategory = _database.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (checkExistenceSubCategory.Count() > 0)
                {
                    StatusMessage = "Error: de ingevoerde subcategorie bestaat al in " + checkExistenceSubCategory.First().Category.Name + "! Gebruik een andere naam.";
                }
                else
                {
                    var retrievedSubCategory = await _database.SubCategory.FindAsync(model.SubCategory.Id);
                    retrievedSubCategory.Name = model.SubCategory.Name;
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


        //LOAD VIEW: DELETE SUBCATEGORY
        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                NotFound();
            }

            var subCategory = await _database.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                NotFound();
            }

            return View(subCategory);
        }


        //REMOVE: SUBCATEGORY FROM DB
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRemove(int id)
        {
            var subCategory = await _database.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            _database.SubCategory.Remove(subCategory);
            await _database.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}