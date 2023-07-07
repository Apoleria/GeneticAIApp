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
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subject
        public async Task<IActionResult> Index()
        {
              return _context.SubjectViewModel != null ? 
                          View(await _context.SubjectViewModel.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.SubjectViewModel'  is null.");
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SubjectViewModel == null)
            {
                return NotFound();
            }

            var subjectViewModel = await _context.SubjectViewModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (subjectViewModel == null)
            {
                return NotFound();
            }

            return View(subjectViewModel);
        }

        // GET: Subject/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subject/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,CourseName")] SubjectViewModel subjectViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subjectViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subjectViewModel);
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SubjectViewModel == null)
            {
                return NotFound();
            }

            var subjectViewModel = await _context.SubjectViewModel.FindAsync(id);
            if (subjectViewModel == null)
            {
                return NotFound();
            }
            return View(subjectViewModel);
        }

        // POST: Subject/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,CourseName")] SubjectViewModel subjectViewModel)
        {
            if (id != subjectViewModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subjectViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectViewModelExists(subjectViewModel.id))
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
            return View(subjectViewModel);
        }

        // GET: Subject/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SubjectViewModel == null)
            {
                return NotFound();
            }

            var subjectViewModel = await _context.SubjectViewModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (subjectViewModel == null)
            {
                return NotFound();
            }

            return View(subjectViewModel);
        }

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SubjectViewModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SubjectViewModel'  is null.");
            }
            var subjectViewModel = await _context.SubjectViewModel.FindAsync(id);
            if (subjectViewModel != null)
            {
                _context.SubjectViewModel.Remove(subjectViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectViewModelExists(int id)
        {
          return (_context.SubjectViewModel?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
