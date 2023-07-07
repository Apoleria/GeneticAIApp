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
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Teacher
        public async Task<IActionResult> Index()
        {
              return _context.TeacherViewModel != null ? 
                          View(await _context.TeacherViewModel.OrderBy(a => a.Name).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.TeacherViewModel'  is null.");
        }

        // GET: Teacher/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TeacherViewModel == null)
            {
                return NotFound();
            }

            var teacherViewModel = await _context.TeacherViewModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (teacherViewModel == null)
            {
                return NotFound();
            }

            return View(teacherViewModel);
        }

        // GET: Teacher/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teacher/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Name")] TeacherViewModel teacherViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacherViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacherViewModel);
        }

        // GET: Teacher/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TeacherViewModel == null)
            {
                return NotFound();
            }

            var teacherViewModel = await _context.TeacherViewModel.FindAsync(id);
            if (teacherViewModel == null)
            {
                return NotFound();
            }
            return View(teacherViewModel);
        }

        // POST: Teacher/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Name")] TeacherViewModel teacherViewModel)
        {
            if (id != teacherViewModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacherViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherViewModelExists(teacherViewModel.id))
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
            return View(teacherViewModel);
        }

        // GET: Teacher/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TeacherViewModel == null)
            {
                return NotFound();
            }

            var teacherViewModel = await _context.TeacherViewModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (teacherViewModel == null)
            {
                return NotFound();
            }

            return View(teacherViewModel);
        }

        // POST: Teacher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TeacherViewModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TeacherViewModel'  is null.");
            }
            var teacherViewModel = await _context.TeacherViewModel.FindAsync(id);
            if (teacherViewModel != null)
            {
                _context.TeacherViewModel.Remove(teacherViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherViewModelExists(int id)
        {
          return (_context.TeacherViewModel?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
