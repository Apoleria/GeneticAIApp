using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GeneticAIApp.Data;
using GeneticAIApp.Models;

namespace GeneticAIApp.Controllers
{
    public class GradeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private GradeViewModel grViewModel;

        public GradeController(ApplicationDbContext context)
        {
            _context = context;
            grViewModel = new GradeViewModel();
        }

        // GET: Grade
        public async Task<IActionResult> Index()
        {

            ViewData["gradeViewModel"] = grViewModel;
            return _context.GradeViewModel != null ? 
                          View(await _context.GradeViewModel.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.GradeViewModel'  is null.");
        }

        // GET: Grade/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["gradeViewModel"] = grViewModel;
            if (id == null || _context.GradeViewModel == null)
            {
                return NotFound();
            }

            var gradeViewModel = await _context.GradeViewModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (gradeViewModel == null)
            {
                return NotFound();
            }

            return View(gradeViewModel);
        }

        // GET: Grade/Create
        public IActionResult Create()
        {
            int[] numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewData["Number"] = numbers.Select(item => new SelectListItem
            {
                Text = item.ToString(),
                Value = item.ToString()
            });
            ViewData["gradeViewModel"] = grViewModel;
            return View();
        }

        // POST: Grade/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Letter,Number,Group")] GradeViewModel gradeViewModel)
        {
            //ViewData["gradeViewModel"] = grViewModel;
            if (ModelState.IsValid)
            {
                _context.Add(gradeViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["gradeViewModel"] = gradeViewModel;
            return View(gradeViewModel);
        }

        // GET: Grade/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //ViewData["gradeViewModel"] = grViewModel;
            if (id == null || _context.GradeViewModel == null)
            {
                return NotFound();
            }

            var gradeViewModel = await _context.GradeViewModel.FindAsync(id);
            if (gradeViewModel == null)
            {
                return NotFound();
            }
            int[] numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewData["Number"] = numbers.Select(item => new SelectListItem
            {
                Text = item.ToString(),
                Value = item.ToString(),
                Selected = true
            });
            ViewData["gradeViewModel"] = gradeViewModel;
            return View(gradeViewModel);
        }

        // POST: Grade/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Letter,Number,Group")] GradeViewModel gradeViewModel)
        {
            ViewData["gradeViewModel"] = grViewModel;
            if (id != gradeViewModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gradeViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradeViewModelExists(gradeViewModel.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gradeViewModel);
        }

        // GET: Grade/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["gradeViewModel"] = grViewModel;
            if (id == null || _context.GradeViewModel == null)
            {
                return NotFound();
            }

            var gradeViewModel = await _context.GradeViewModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (gradeViewModel == null)
            {
                return NotFound();
            }

            return View(gradeViewModel);
        }

        // POST: Grade/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["gradeViewModel"] = grViewModel;
            if (_context.GradeViewModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GradeViewModel'  is null.");
            }
            var gradeViewModel = await _context.GradeViewModel.FindAsync(id);
            if (gradeViewModel != null)
            {
                _context.GradeViewModel.Remove(gradeViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradeViewModelExists(int id)
        {
          return (_context.GradeViewModel?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
