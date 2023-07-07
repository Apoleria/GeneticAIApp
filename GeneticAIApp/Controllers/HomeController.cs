using GeneticAIApp.Data;
using GeneticAIApp.Models;
using GeneticAIApp.GenticAlgo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeneticAIApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private GeneticIndex genIndex;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            genIndex = new(_context);
        }

        public IActionResult Index()
        {
/*            if (round != null)
            {
                //genIndex.GenerateChild();
                //return RedirectToAction(nameof(Index), round);
                //ViewData["ParentsGeneration"] = "Генерирани са родителските програми";
            }*/
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Index(string submit)
        {
            
            genIndex.Start();
            genIndex.GenerateChild();
            //ViewData["ParentsGeneration"] = "Генерирани са родителските програми";
            return RedirectToAction(nameof(Program));

            //return View();
        }

        public IActionResult Program()
        {
            
            var finalProgram = genIndex.getFinalProgram();
            List<SelectListItem> grades = new List<SelectListItem>();

            if(finalProgram is not null)
            {
                List<GradeViewModel> allGrades = new();
                foreach (var item in finalProgram.fullProgram)
                {
                    allGrades.Add(item.Grade);
                }

                ViewData["grade"] = allGrades.Select(item => new SelectListItem
                {
                    Text = item.FullName(),
                    Value = item.id.ToString()
                });
                return View(finalProgram.fullProgram);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Program(string grade)
        {

            var finalProgram = genIndex.getFinalProgram();
            List<SelectListItem> grades = new List<SelectListItem>();


            List<GradeViewModel> allGrades = new();
            foreach (var item in finalProgram.fullProgram)
            {
                allGrades.Add(item.Grade);
            }

            ViewData["grade"] = allGrades.Select(item => new SelectListItem
            {
                Text = item.FullName(),
                Value = item.id.ToString()
            });

            List<SingleGradeWeekProgram> lsgwp = new List<SingleGradeWeekProgram>();
            foreach (var item in finalProgram.fullProgram)
            {
                if (item.Grade.id == int.Parse(grade))
                {
                    lsgwp.Add(item);
                }
            }

            return View(lsgwp);
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}