using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GeneticAIApp.Data;
using GeneticAIApp.Models;
using System.Configuration;

namespace GeneticAIApp.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            if (_context.CoursesViewModel is not null) 
            {
                var applicationDbContext = _context.CoursesViewModel.Include(c => c.Grade).Include(c => c.Subject).Include(c => c.Teacher);
                return View(await applicationDbContext.ToListAsync());
            }
            return View();
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? subjectId, int? gradeId, int? teacherid)
        {
            if (subjectId == null || gradeId == null || teacherid == null || _context.CoursesViewModel is null)
            {
                return NotFound();
            }

            var coursesViewModel = await _context.CoursesViewModel
                .Include(c => c.Grade)
                .Include(c => c.Subject)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.GradeId == gradeId && m.SubjectId == subjectId && m.Teacherid == teacherid);
            if (coursesViewModel == null)
            {
                return NotFound();
            }

            return View(coursesViewModel);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            var allGrades = _context.GradeViewModel.Select(x => x).ToList();
            //ViewData["GradeId"] = new SelectList(_context.GradeViewModel, "id", "Number");
            ViewData["GradeId"] = allGrades.Select(item => new SelectListItem
            {
                Text = item.FullName(),
                Value = item.id.ToString()
            });
            ViewData["SubjectId"] = new SelectList(_context.SubjectViewModel, "id", "CourseName");
            ViewData["Teacherid"] = new SelectList(_context.TeacherViewModel, "id", "Name");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("GradeId,SubjectId,Teacherid,ForWeekId")] CoursesViewModel coursesViewModel)
        public async Task<IActionResult> Create(string gradeId, string subjectId, string teacherid, string ForWeekId)
        {
            CoursesViewModel coursesViewModel = new CoursesViewModel(Int32.Parse(gradeId), Int32.Parse(subjectId), Int32.Parse(teacherid), _context);
            if (gradeId != null && subjectId != null && teacherid != null && ForWeekId != null)
            {
                coursesViewModel.GradeId = Int32.Parse(gradeId);
                coursesViewModel.SubjectId = Int32.Parse(subjectId);
                coursesViewModel.Teacherid = Int32.Parse(teacherid);
                coursesViewModel.ForWeekId = Int32.Parse(ForWeekId);

                if (!CoursesViewModelExists(coursesViewModel.GradeId, coursesViewModel.SubjectId, coursesViewModel.Teacherid))
                {
                    try
                    {
                        _context.Add(coursesViewModel);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        TempData["Message"] = "Възникна грешка в базата";
                    }
                    
                } else
                {
                    //_context.Update(coursesViewModel);
                    TempData["Message"] = "Този час вече е добавен";
                }

                return RedirectToAction(nameof(Index));
            }
/*            if (ModelState.IsValid)
            {
                _context.Add(coursesViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }*/
            ViewData["GradeId"] = new SelectList(_context.GradeViewModel, "id", "Letter", coursesViewModel.GradeId);
            ViewData["SubjectId"] = new SelectList(_context.SubjectViewModel, "id", "CourseName", coursesViewModel.SubjectId);
            ViewData["Teacherid"] = new SelectList(_context.TeacherViewModel, "id", "Name", coursesViewModel.Teacherid);
            return View(coursesViewModel);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? gradeId, int? subjectId, int? teacherid)
        {
            if (subjectId == null || gradeId == null || teacherid == null || _context.CoursesViewModel is null)
            {
                return NotFound();
            }

            var coursesViewModel = await _context.CoursesViewModel.FindAsync(subjectId, gradeId, teacherid);
            if (coursesViewModel == null)
            {
                return NotFound();
            }
            ViewData["GradeId"] = new SelectList(_context.GradeViewModel, "id", "Letter", coursesViewModel.GradeId);
            ViewData["SubjectId"] = new SelectList(_context.SubjectViewModel, "id", "CourseName", coursesViewModel.SubjectId);
            ViewData["Teacherid"] = new SelectList(_context.TeacherViewModel, "id", "Name", coursesViewModel.Teacherid);
            return View(coursesViewModel);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string gradeId, string subjectId, string teacherid, string ForWeekId)
        {
            //[Bind("GradeId,SubjectId,Teacherid,ForWeekId")] CoursesViewModel coursesViewModel беше в параметрите
            CoursesViewModel coursesViewModelMy = new CoursesViewModel(Int32.Parse(gradeId), Int32.Parse(subjectId), Int32.Parse(teacherid), _context);

            /*            if (Int32.Parse(subjectId) != coursesViewModel.SubjectId && Int32.Parse(gradeId) != coursesViewModel.GradeId && Int32.Parse(teacherid) != coursesViewModel.Teacherid)
                        {
                            return NotFound();
                        }*/
            if (Int32.Parse(gradeId) != 0 && Int32.Parse(subjectId) != 0 && Int32.Parse(teacherid) != 0 && Int32.Parse(ForWeekId) != 0)
            {
                coursesViewModelMy.GradeId = Int32.Parse(gradeId);
                coursesViewModelMy.SubjectId = Int32.Parse(subjectId);
                coursesViewModelMy.Teacherid = Int32.Parse(teacherid);
                coursesViewModelMy.ForWeekId = Int32.Parse(ForWeekId);

                if (CoursesViewModelExists(coursesViewModelMy.GradeId, coursesViewModelMy.SubjectId, coursesViewModelMy.Teacherid))
                {
                    _context.Update(coursesViewModelMy);
                    await _context.SaveChangesAsync();
                } else
                {
                    TempData["Message"] = "Няма такъв час";
                }
                return RedirectToAction(nameof(Index));
            }

            
/*

            if (CoursesViewModelExists(coursesViewModelMy.GradeId, coursesViewModelMy.SubjectId, coursesViewModelMy.Teacherid))
            {
                try
                {
                    _context.Update(coursesViewModelMy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }

                if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coursesViewModelMy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoursesViewModelExists(coursesViewModel.GradeId, coursesViewModel.SubjectId, coursesViewModel.Teacherid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }*/
            ViewData["GradeId"] = new SelectList(_context.GradeViewModel, "id", "Letter", coursesViewModelMy.GradeId);
            ViewData["SubjectId"] = new SelectList(_context.SubjectViewModel, "id", "CourseName", coursesViewModelMy.SubjectId);
            ViewData["Teacherid"] = new SelectList(_context.TeacherViewModel, "id", "Name", coursesViewModelMy.Teacherid);
            return View(coursesViewModelMy);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? subjectId, int? gradeId, int? teacherid)
        {
            if (subjectId == null || gradeId == null || teacherid == null || _context.CoursesViewModel is null)
            {
                return NotFound();
            }

            var coursesViewModel = await _context.CoursesViewModel
                .Include(c => c.Grade)
                .Include(c => c.Subject)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.SubjectId == subjectId && m.GradeId == gradeId && m.Teacherid == teacherid);
            if (coursesViewModel == null)
            {
                return NotFound();
            }

            return View(coursesViewModel);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int subjectId, int gradeId, int teacherid)
        {
            if (_context.CoursesViewModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CoursesViewModel'  is null.");
            }
            var coursesViewModel = await _context.CoursesViewModel.FindAsync(subjectId, gradeId, teacherid);
            if (coursesViewModel != null)
            {
                _context.CoursesViewModel.Remove(coursesViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoursesViewModelExists(int gradeId, int subjectId, int teacherid)
        {
          return (_context.CoursesViewModel?.Any(e => e.GradeId == gradeId && e.SubjectId == subjectId && e.Teacherid == teacherid)).GetValueOrDefault();
        }
    }
}
