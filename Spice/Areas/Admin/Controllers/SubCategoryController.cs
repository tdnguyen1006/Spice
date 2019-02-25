using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.SubCategory.Include(s => s.Category).ToListAsync());
        }

        #region Create Sub-Category
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync()
            };

            if (!ModelState.IsValid)
            {
                return View(modelVM);
            }

            var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);

            if(doesSubCategoryExists.Count() > 0)
            {
                StatusMessage = "Error: Sub-category already exists under " + doesSubCategoryExists.First().Category.Name + " category. Please enter another...";
                modelVM.StatusMessage = StatusMessage;
                return View(modelVM);
            }
            else
            {
                _db.SubCategory.Add(model.SubCategory);
                await _db.SaveChangesAsync();
                StatusMessage = "Sub-category " + model.SubCategory.Name + " has been created successfully!";
                modelVM.StatusMessage = StatusMessage;
                return RedirectToAction(nameof(Index));
            }
            
        }
        #endregion Create Sub-Category

        #region Edit Sub-Category
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync()
            };

            if (!ModelState.IsValid)
            {
                return View(modelVM);
            }

            var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);

            if (doesSubCategoryExists.Count() > 0)
            {
                StatusMessage = "Error: Sub-category already exists under " + doesSubCategoryExists.First().Category.Name + " category. Please enter another...";
                modelVM.StatusMessage = StatusMessage;
                return View(modelVM);
            }
            else
            {
                var subCatFromDb = await _db.SubCategory.FindAsync(id);
                subCatFromDb.Name = model.SubCategory.Name;
                
                await _db.SaveChangesAsync();
                StatusMessage = "Sub-category " + model.SubCategory.Name + " has been edited successfully!";
                modelVM.StatusMessage = StatusMessage;
                return RedirectToAction(nameof(Index));
            }

        }
        #endregion Create

        public async Task<IActionResult> Delete(int id)
        {
            var subcategory = await _db.SubCategory.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (subcategory == null)
            {
                return NotFound();
            }
            _db.SubCategory.Remove(subcategory);
            await _db.SaveChangesAsync();
            TempData["Message"] = "Delete successfully!";
            return RedirectToAction(nameof(Index));
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }
    }
}