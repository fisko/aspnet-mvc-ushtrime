using University.Models;

namespace University.Data;

// Mbush bazën me të dhëna shembull HERËN E PARË që niset aplikacioni.
// Thirret nga Program.cs pas db.Database.Migrate().
public static class SeedData
{
    public static void Initialize(UniversityContext db)
    {
        // Nëse ka tashmë fakultete, baza është mbushur -> mos shto sërish.
        if (db.Faculties.Any())
        {
            return;
        }

        // Ndërtojmë një "pemë" objektesh: Fakultet -> Departamente -> Profesorë.
        // EF Core i ruan të gjitha me një Add(), dhe i plotëson vetë çelësat e huaj.
        var fshi = new Faculty
        {
            Name = "Fakulteti i Shkencave të Informatikës",
            Departments =
            [
                new Department
                {
                    Name = "Inxhinieri Softuerike",
                    Professors =
                    [
                        new Professor { FirstName = "Arben", LastName = "Krasniqi", Email = "arben.krasniqi@uni.test" },
                        new Professor { FirstName = "Elira", LastName = "Berisha", Email = "elira.berisha@uni.test" }
                    ]
                },
                new Department
                {
                    Name = "Rrjeta dhe Siguri",
                    Professors =
                    [
                        new Professor { FirstName = "Besnik", LastName = "Hoxha", Email = "besnik.hoxha@uni.test" }
                    ]
                }
            ]
        };
        db.Faculties.Add(fshi);
        db.SaveChanges(); // pas kësaj, çdo objekt ka marrë Id nga SQL Server

        var se = fshi.Departments[0];
        var net = fshi.Departments[1];

        var courses = new[]
        {
            new Course { Code = "PV101", Title = "Programim Vizual", Credits = 6, DepartmentId = se.Id, ProfessorId = se.Professors[0].Id },
            new Course { Code = "BP201", Title = "Baza e të Dhënave", Credits = 6, DepartmentId = se.Id, ProfessorId = se.Professors[1].Id },
            new Course { Code = "RK301", Title = "Rrjeta Kompjuterike", Credits = 5, DepartmentId = net.Id, ProfessorId = net.Professors[0].Id }
        };
        db.Courses.AddRange(courses);

        // Email-i i Anës përputhet me llogarinë "ana.gashi@student.test" te IdentitySeed.
        var students = new[]
        {
            new Student { FirstName = "Ana", LastName = "Gashi", StudentNumber = "2024/001", Email = "ana.gashi@student.test" },
            new Student { FirstName = "Dren", LastName = "Morina", StudentNumber = "2024/002", Email = "dren.morina@student.test" },
            new Student { FirstName = "Liridona", LastName = "Shala", StudentNumber = "2024/003", Email = "liridona.shala@student.test" }
        };
        db.Students.AddRange(students);
        db.SaveChanges();

        // Regjistrimet: Ana ka 10 dhe 9 -> mesatarja 9.5. Liridona ende pa notë.
        db.Enrollments.AddRange(
            new Enrollment { StudentId = students[0].Id, CourseId = courses[0].Id, Grade = 10 },
            new Enrollment { StudentId = students[0].Id, CourseId = courses[1].Id, Grade = 9 },
            new Enrollment { StudentId = students[1].Id, CourseId = courses[0].Id, Grade = 8 },
            new Enrollment { StudentId = students[2].Id, CourseId = courses[2].Id });
        db.SaveChanges();
    }
}
