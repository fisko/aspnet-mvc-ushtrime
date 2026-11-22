using System.Data.Entity.Migrations;
using System.Linq;
using UniversityApi.Models;

namespace UniversityApi.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<UniversityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UniversityContext context)
        {
            context.Students.AddOrUpdate(s => s.Email,
                new Student { FirstName = "Arben", LastName = "Krasniqi", Email = "arben.krasniqi@example.com" },
                new Student { FirstName = "Elira", LastName = "Berisha", Email = "elira.berisha@example.com" },
                new Student { FirstName = "Dren", LastName = "Ahmeti", Email = "dren.ahmeti@example.com" });

            context.Courses.AddOrUpdate(c => c.Title,
                new Course { Title = "ASP.NET MVC", Credits = 6 },
                new Course { Title = "Baza e të dhënave", Credits = 5 },
                new Course { Title = "Programim vizual", Credits = 6 });

            // Ruaj, që studentët dhe kurset të marrin Id para se të lidhen
            context.SaveChanges();

            var arben = context.Students.First(s => s.Email == "arben.krasniqi@example.com");
            var elira = context.Students.First(s => s.Email == "elira.berisha@example.com");
            var mvc = context.Courses.First(c => c.Title == "ASP.NET MVC");
            var dbCourse = context.Courses.First(c => c.Title == "Baza e të dhënave");

            context.Enrollments.AddOrUpdate(e => new { e.StudentId, e.CourseId },
                new Enrollment { StudentId = arben.Id, CourseId = mvc.Id, Grade = 9 },
                new Enrollment { StudentId = arben.Id, CourseId = dbCourse.Id },
                new Enrollment { StudentId = elira.Id, CourseId = mvc.Id, Grade = 10 });

            context.SaveChanges();
        }
    }
}
