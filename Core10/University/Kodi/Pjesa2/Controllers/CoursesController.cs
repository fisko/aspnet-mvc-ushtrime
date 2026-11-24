using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;

namespace University.Controllers;

// Controller MVC (kthen HTML me Razor View) - krahasoni me CoursesApiController (kthen JSON).
// Trashëgon nga Controller (ka View()), jo nga ControllerBase.
public class CoursesController(UniversityContext db) : Controller
{
    // GET /Courses - publike
    public async Task<IActionResult> Index()
    {
        var courses = await db.Courses
            .AsNoTracking()
            .Include(c => c.Department)   // pa Include, c.Department do ishte null në View
            .Include(c => c.Professor)
            .OrderBy(c => c.Code)
            .ToListAsync();
        return View(courses); // -> Views/Courses/Index.cshtml me @model List<Course>
    }

    // GET /Courses/Create - shfaq formën bosh (vetëm Admin)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadListsAsync();
        return View();
    }

    // POST /Courses/Create - përpunon formën
    // [Bind] lejon VETËM këto fusha -> "overposting" (p.sh. Id) s'kalon.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Code,Title,Credits,DepartmentId,ProfessorId")] Course course)
    {
        // Validim që kërkon bazën -> e shtojmë manualisht te ModelState.
        if (await db.Courses.AnyAsync(c => c.Code == course.Code))
        {
            ModelState.AddModelError(nameof(Course.Code), "Ky kod ekziston tashmë.");
        }

        if (!ModelState.IsValid)
        {
            await LoadListsAsync(); // dropdown-et duhen rimbushur
            return View(course);
        }

        db.Courses.Add(course);
        await db.SaveChangesAsync();
        // PRG (Post-Redirect-Get): pas POST-it ridrejtojmë, që F5 të mos e dërgojë formën sërish.
        return RedirectToAction(nameof(Index));
    }

    // Mbush listat për <select> (departamentet, profesorët). ViewBag = "qese" dinamike për View-n.
    private async Task LoadListsAsync()
    {
        ViewBag.Departments = new SelectList(
            await db.Departments.AsNoTracking().OrderBy(d => d.Name).ToListAsync(), "Id", "Name");
        ViewBag.Professors = new SelectList(
            await db.Professors.AsNoTracking().OrderBy(p => p.LastName).ToListAsync(), "Id", "FullName");
    }
}
