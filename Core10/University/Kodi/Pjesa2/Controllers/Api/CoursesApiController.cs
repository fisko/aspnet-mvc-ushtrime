// =====================================================================
// CoursesApiController - Web API për kurset (kthen JSON, jo HTML)
// ---------------------------------------------------------------------
//   GET    /api/courses        -> lista (publike)
//   GET    /api/courses/5      -> një kurs (publike)
//   POST   /api/courses        -> krijo   (vetëm Admin)
//   PUT    /api/courses/5      -> ndrysho (vetëm Admin)
//   DELETE /api/courses/5      -> fshi    (vetëm Admin)
//
// [ApiController] jep automatikisht:
//   - 400 + ProblemDetails nëse DTO-ja s'kalon validimin
//   - lidhjen e body-t JSON me parametrin (pa [FromBody])
// ControllerBase (jo Controller) -> s'ka View, vetëm të dhëna.
// =====================================================================

using System.Linq.Expressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Dtos;
using University.Models;

namespace University.Controllers.Api;

[ApiController]
[Route("api/courses")]
// Për API-në përdorim JWT (jo cookie). Pa këtë, 401 do bëhej redirect te faqja Login.
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
// DbContext-i injektohet nga DI (primary constructor).
public class CoursesApiController(UniversityContext db) : ControllerBase
{
    // Projeksioni Course -> CourseDto, i shkruar NJË herë dhe i ripërdorur.
    // Si Expression, EF Core e kthen në SQL (SELECT vetëm kolonat që duhen + JOIN).
    private static readonly Expression<Func<Course, CourseDto>> ToDto = c => new CourseDto(
        c.Id, c.Code, c.Title, c.Credits,
        c.Department!.Name,
        c.Professor == null ? null : c.Professor.FirstName + " " + c.Professor.LastName);

    [HttpGet]
    [AllowAnonymous] // kushdo mund ta shohë listën e kurseve
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
    {
        // AsNoTracking: vetëm lexim -> më shpejt (EF s'i ndjek ndryshimet).
        return await db.Courses
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .Select(ToDto)
            .ToListAsync();
    }

    // {id:int} = "route constraint": /api/courses/abc s'përputhet fare -> 404.
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CourseDto>> GetById(int id)
    {
        var course = await db.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(ToDto)
            .FirstOrDefaultAsync();
        return course is null ? NotFound() : course;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // pa token -> 401, me token jo-Admin -> 403
    public async Task<ActionResult<CourseDto>> Create(CourseInputDto input)
    {
        // Kontrollet që validimi me atribute s'i bën dot (kërkojnë bazën).
        var problem = await ValidateAsync(input, currentId: null);
        if (problem is not null)
        {
            return problem;
        }

        var course = new Course
        {
            Code = input.Code,
            Title = input.Title,
            Credits = input.Credits,
            DepartmentId = input.DepartmentId,
            ProfessorId = input.ProfessorId
        };
        db.Courses.Add(course);
        await db.SaveChangesAsync(); // INSERT; pas kësaj course.Id ka vlerë

        var dto = (await GetById(course.Id)).Value;
        // 201 Created + header "Location: /api/courses/{id}"
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CourseInputDto input)
    {
        // FindAsync kërkon sipas çelësit primar. Objekti "ndiqet" (tracked) nga EF.
        var course = await db.Courses.FindAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        var problem = await ValidateAsync(input, currentId: id);
        if (problem is not null)
        {
            return problem;
        }

        course.Code = input.Code;
        course.Title = input.Title;
        course.Credits = input.Credits;
        course.DepartmentId = input.DepartmentId;
        course.ProfessorId = input.ProfessorId;
        await db.SaveChangesAsync(); // EF e di çfarë ndryshoi -> UPDATE vetëm ato kolona
        return NoContent();          // 204: u krye, s'ka përmbajtje për të kthyer
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        // Pa këtë kontroll, EF do fshinte (cascade) edhe regjistrimet DHE notat!
        if (await db.Enrollments.AnyAsync(e => e.CourseId == id))
        {
            return Problem(detail: "Kursi ka studentë të regjistruar dhe nuk mund të fshihet.",
                statusCode: StatusCodes.Status409Conflict);
        }

        db.Courses.Remove(course);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // Kthen null nëse gjithçka është në rregull, përndryshe një përgjigje gabimi.
    // Problem(...) kthen JSON standard "ProblemDetails" (RFC 9457).
    private async Task<ActionResult?> ValidateAsync(CourseInputDto input, int? currentId)
    {
        if (!await db.Departments.AnyAsync(d => d.Id == input.DepartmentId))
        {
            return Problem(detail: "Departamenti nuk ekziston.", statusCode: StatusCodes.Status400BadRequest);
        }

        if (input.ProfessorId is not null && !await db.Professors.AnyAsync(p => p.Id == input.ProfessorId))
        {
            return Problem(detail: "Profesori nuk ekziston.", statusCode: StatusCodes.Status400BadRequest);
        }

        // Pa këtë, indeksi unik në SQL do hidhte exception -> 500.
        if (await db.Courses.AnyAsync(c => c.Code == input.Code && c.Id != currentId))
        {
            return Problem(detail: $"Kodi '{input.Code}' ekziston tashmë.", statusCode: StatusCodes.Status409Conflict);
        }

        return null;
    }
}
