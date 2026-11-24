using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Dtos;
using University.Models;

namespace University.Controllers.Api;

// /api/enrollments - regjistrimi në kurse dhe vendosja e notave.
// Pjesa 1: KUSHDO mund të regjistrojë KËDO dhe të vendosë nota!
// Kjo është qëllimisht e pasigurt - në Pjesën 2 e rregullojmë me role dhe JWT.
[ApiController]
[Route("api/enrollments")]
public class EnrollmentsApiController(UniversityContext db) : ControllerBase
{
    // POST /api/enrollments  { "studentId": 1, "courseId": 3 }
    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> Enroll(EnrollInputDto input)
    {
        var course = await db.Courses.FindAsync(input.CourseId);
        var student = await db.Students.FindAsync(input.StudentId);
        if (course is null || student is null)
        {
            return Problem(detail: "Studenti ose kursi nuk ekziston.", statusCode: StatusCodes.Status400BadRequest);
        }

        if (await db.Enrollments.AnyAsync(e => e.StudentId == input.StudentId && e.CourseId == input.CourseId))
        {
            // 409 Conflict = kërkesa bie ndesh me gjendjen aktuale
            return Problem(detail: "Studenti është i regjistruar tashmë në këtë kurs.", statusCode: StatusCodes.Status409Conflict);
        }

        var enrollment = new Enrollment { StudentId = input.StudentId, CourseId = input.CourseId };
        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        return Created($"/api/enrollments/{enrollment.Id}", new EnrollmentDto(enrollment.Id, course.Title, null));
    }

    // PUT /api/enrollments/1/grade  { "grade": 9 }
    [HttpPut("{id:int}/grade")]
    public async Task<IActionResult> SetGrade(int id, GradeInputDto input)
    {
        var enrollment = await db.Enrollments.FindAsync(id);
        if (enrollment is null)
        {
            return NotFound();
        }

        enrollment.Grade = input.Grade;
        await db.SaveChangesAsync();
        return NoContent();
    }
}
