using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Services;

namespace University.Controllers;

// Dy varësi nga DI: DbContext-i dhe shërbimi i notave (IGradeService).
// Pjesa 1: publik. Në Pjesën 2 shtojmë [Authorize] (vetëm të kyçurit).
public class StudentsController(UniversityContext db, IGradeService grades) : Controller
{
    // GET /Students
    public async Task<IActionResult> Index()
    {
        var students = await db.Students.AsNoTracking().OrderBy(s => s.LastName).ToListAsync();
        return View(students);
    }

    // GET /Students/Details/1  ("id" vjen nga rruga {controller}/{action}/{id?})
    public async Task<IActionResult> Details(int id)
    {
        var student = await db.Students
            .AsNoTracking()
            .Include(s => s.Enrollments).ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student is null)
        {
            return NotFound();
        }

        // I njëjti shërbim si te API-ja -> logjika s'përsëritet.
        ViewBag.Average = grades.Average(student.Enrollments.Select(e => e.Grade));
        return View(student);
    }
}
