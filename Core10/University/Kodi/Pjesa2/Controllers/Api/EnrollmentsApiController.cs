using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Dtos;
using University.Models;

namespace University.Controllers.Api;

// /api/enrollments - regjistrimi në kurse dhe vendosja e notave.
[ApiController]
[Route("api/enrollments")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EnrollmentsApiController(UniversityContext db) : ControllerBase
{
    // POST /api/enrollments  { "studentId": 1, "courseId": 3 }
    // Roles = "Admin,Student" -> mjafton NJËRI prej roleve (OR).
    [HttpPost]
    [Authorize(Roles = "Admin,Student")]
    public async Task<ActionResult<EnrollmentDto>> Enroll(EnrollInputDto input)
    {
        var course = await db.Courses.FindAsync(input.CourseId);
        var student = await db.Students.FindAsync(input.StudentId);
        if (course is null || student is null)
        {
            return Problem(detail: "Studenti ose kursi nuk ekziston.", statusCode: StatusCodes.Status400BadRequest);
        }

        // SIGURIA: një Student mund të regjistrojë VETËM VETEN, jo shokun!
        // User.Identity.Name vjen nga tokeni (claim "Name" = email-i i llogarisë).
        // Pa këtë kontroll, Ana mund të dërgonte studentId = 2 dhe të regjistronte Drenin.
        if (!User.IsInRole("Admin") &&
            !string.Equals(student.Email, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid(); // 403
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
    // Policy "GradeEditors" (Program.cs) = Admin OSE Professor.
    [HttpPut("{id:int}/grade")]
    [Authorize(Policy = "GradeEditors")]
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
