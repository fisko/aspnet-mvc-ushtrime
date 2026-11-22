using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Description;
using UniversityApi.Models;

namespace UniversityApi.Controllers
{
    public class EnrollmentsController : ApiController
    {
        private UniversityContext db = new UniversityContext();

        // Si kthehet një Enrollment në EnrollmentDto (përdoret në disa vende)
        private static readonly Expression<Func<Enrollment, EnrollmentDto>> AsDto =
            e => new EnrollmentDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = e.Student.FirstName + " " + e.Student.LastName,
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                Grade = e.Grade
            };

        // GET: api/Enrollments
        public IQueryable<EnrollmentDto> GetEnrollments()
        {
            return db.Enrollments.Select(AsDto);
        }

        // GET: api/Enrollments/5
        [ResponseType(typeof(EnrollmentDto))]
        public IHttpActionResult GetEnrollment(int id)
        {
            var enrollment = db.Enrollments.Where(e => e.Id == id).Select(AsDto).FirstOrDefault();
            if (enrollment == null)
            {
                return NotFound();
            }

            return Ok(enrollment);
        }

        // POST: api/Enrollments
        [ResponseType(typeof(EnrollmentDto))]
        public IHttpActionResult PostEnrollment(Enrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!db.Students.Any(s => s.Id == enrollment.StudentId) ||
                !db.Courses.Any(c => c.Id == enrollment.CourseId))
            {
                return BadRequest("Studenti ose kursi nuk ekziston.");
            }

            if (db.Enrollments.Any(e => e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId))
            {
                return Conflict();
            }

            db.Enrollments.Add(enrollment);
            db.SaveChanges();

            var dto = db.Enrollments.Where(e => e.Id == enrollment.Id).Select(AsDto).First();
            return CreatedAtRoute("DefaultApi", new { id = dto.Id }, dto);
        }

        // DELETE: api/Enrollments/5
        public IHttpActionResult DeleteEnrollment(int id)
        {
            var enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            db.Enrollments.Remove(enrollment);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
