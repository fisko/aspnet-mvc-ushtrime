using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Dtos;
using University.Models;
using University.Services;

namespace University.Controllers.Api;

// /api/students - vetëm për përdorues të kyçur (me token JWT).
// [Authorize] në nivel klase vlen për TË GJITHA metodat më poshtë.
[ApiController]
[Route("api/students")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
// Dy varësi nga DI: DbContext-i dhe shërbimi ynë i notave.
public class StudentsApiController(UniversityContext db, IGradeService grades) : ControllerBase
{
    // GET /api/students
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
    {
        // Select(...) direkt në DTO -> SQL merr vetëm kolonat që duhen.
        return await db.Students
            .AsNoTracking()
            .OrderBy(s => s.LastName)
            .Select(s => new StudentDto(s.Id, s.FirstName + " " + s.LastName, s.StudentNumber, s.Email))
            .ToListAsync();
    }

    // GET /api/students/1 -> detajet, kurset dhe mesatarja
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDetailsDto>> GetById(int id)
    {
        // Include / ThenInclude = "eager loading": ngarko edhe regjistrimet
        // dhe kursin e secilit regjistrim (JOIN në SQL).
        var student = await db.Students
            .AsNoTracking()
            .Include(s => s.Enrollments).ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student is null)
        {
            return NotFound();
        }

        return new StudentDetailsDto(
            student.Id,
            student.FullName,
            student.StudentNumber,
            grades.Average(student.Enrollments.Select(e => e.Grade)), // logjika është te shërbimi
            student.Enrollments.Select(e => new EnrollmentDto(e.Id, e.Course!.Title, e.Grade)).ToList());
    }

    // POST /api/students - vetëm Admin
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StudentDto>> Create(StudentInputDto input)
    {
        // Indeksi unik do hidhte exception (500) - më mirë një 409 i qartë.
        if (await db.Students.AnyAsync(s => s.StudentNumber == input.StudentNumber))
        {
            return Problem(detail: $"Numri i indeksit '{input.StudentNumber}' ekziston tashmë.",
                statusCode: StatusCodes.Status409Conflict);
        }

        var student = new Student
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            StudentNumber = input.StudentNumber,
            Email = input.Email
        };
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var dto = new StudentDto(student.Id, student.FullName, student.StudentNumber, student.Email);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, dto);
    }
}
