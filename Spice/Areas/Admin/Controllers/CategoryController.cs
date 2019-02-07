﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {

            return View(await _db.Category.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _db.Add(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var category = await _db.Category.Where(c => c.Id == id).FirstOrDefaultAsync();
            
            if(category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _db.Update(category);
            await _db.SaveChangesAsync();            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Category.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            TempData["Message"] = "Delete successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}