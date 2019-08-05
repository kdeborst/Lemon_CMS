using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lemon.Data;
using Lemon.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Lemon.Utilities;
using Lemon.Models;

namespace Lemon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _database;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }


        //CONSTRUCTOR
        public MenuItemController(ApplicationDbContext database, IHostingEnvironment hostingEnvironment)
        {
            _database = database;
            _hostingEnvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _database.Category,
                MenuItem = new Models.MenuItem()
            };
        }


        //LOAD VIEW: INDEX - MENU ITEMS OVERVIEW
        public async Task<IActionResult> Index()
        {
            var menuItems = await _database.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItems);
        }


        //LOAD VIEW: CREATE MENU ITEM
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }


        //ADD: MENU ITEM TO DB
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFood()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if(!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _database.MenuItem.Add(MenuItemVM.MenuItem);
            await _database.SaveChangesAsync();

            //UNIQUELY (RE)STORE FOOD IMAGE & ASSIGN FOOD ITEM ID AS IMAGE NAME
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _database.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            //IMAGE UPLOAD VERIFICATIONS
            if(files.Count() > 0)
            {
                var uploads = Path.Combine(webRootPath,"img");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads,MenuItemVM.MenuItem.Id + extension),FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }

                menuItemFromDb.Image = @"\img\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath,@"img\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\img\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\img\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _database.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        //LOAD VIEW: DETAILS MENU ITEM
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _database.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if(MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }


        //LOAD VIEW: EDIT MENU ITEM
        public async Task<IActionResult> Change(int ? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _database.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _database.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if(MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }


        //CHANGE: MENU ITEM IN DB
        [HttpPost, ActionName("Change")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeFood(int ? id)
        {

            if(id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = await _database.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }

            //(RE)STORE FOOD IMAGE & ASSIGN FOOD ITEM ID TO IMAGE NAME
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _database.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            //IMAGE UPLOAD VERIFICATIONS
            if (files.Count() > 0)
            {
                //UPLOAD: SUCCESSFUL
                var uploads = Path.Combine(webRootPath, "img");
                var extension_new = Path.GetExtension(files[0].FileName);

                //DELETE ORIGINAL DATA FROM DB
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //UPLOAD NEW IMAGE-PATH
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }

                menuItemFromDb.Image = @"\img\" + MenuItemVM.MenuItem.Id + extension_new;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Spiciness = MenuItemVM.MenuItem.Spiciness;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            await _database.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        //LOAD VIEW: REMOVE FOOD ITEM
        public async Task<IActionResult> Remove(int ? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _database.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }


        //REMOVE: FOOD ITEM FROM DB
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRemove(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            MenuItem menuItem = await _database.MenuItem.FindAsync(id);

            if(menuItem != null)
            {
                var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));

                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _database.MenuItem.Remove(menuItem);
                await _database.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


    }
}