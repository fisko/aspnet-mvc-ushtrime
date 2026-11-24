// =====================================================================
// ENTITETET (Modeli i domenit)
// ---------------------------------------------------------------------
// Çdo klasë këtu bëhet një TABELË në SQL Server (EF Core "Code First").
// Çdo property bëhet KOLONË. Rregullat e EF Core sipas konventës:
//   - "Id" -> çelësi primar (PRIMARY KEY, IDENTITY)
//   - "FacultyId" + "Faculty" -> çelës i huaj (FOREIGN KEY) drejt Faculties
//   - List<...> -> anët "shumë" të lidhjes 1:N (navigation property)
// Atributet [Required], [StringLength], [Range] përdoren njëkohësisht për:
//   1) validim (ModelState në MVC / 400 automatik në API)
//   2) skemën e bazës (NOT NULL, nvarchar(100), ...)
// =====================================================================

using System.ComponentModel.DataAnnotations;

namespace University.Models;

// Fakulteti ka shumë departamente (1:N).
public class Faculty
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    // "= []" inicializon listën bosh (sintaksë e re e C# 12 - collection expression).
    public List<Department> Departments { get; set; } = [];
}

// Departamenti i takon NJË fakulteti dhe ka shumë profesorë e kurse.
public class Department
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    // Çelësi i huaj + navigation property drejt Faculty.
    public int FacultyId { get; set; }
    public Faculty? Faculty { get; set; }   // "?" = mund të jetë null nëse nuk e ngarkojmë me Include()

    public List<Professor> Professors { get; set; } = [];
    public List<Course> Courses { get; set; } = [];
}

public class Professor
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string FirstName { get; set; } = "";

    [Required, StringLength(50)]
    public string LastName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public List<Course> Courses { get; set; } = [];

    // Property e llogaritur (vetëm "get") -> EF Core NUK e ruan në bazë.
    public string FullName => $"{FirstName} {LastName}";
}

public class Course
{
    public int Id { get; set; }

    // Kodi i kursit (p.sh. "PV101") - unik, shih UniversityContext.
    [Required, StringLength(10)]
    public string Code { get; set; } = "";

    [Required, StringLength(100)]
    public string Title { get; set; } = "";

    [Range(1, 12)]
    public int Credits { get; set; }   // kreditet ECTS

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    // "int?" = opsional: një kurs mund të mos ketë ende profesor.
    public int? ProfessorId { get; set; }
    public Professor? Professor { get; set; }

    public List<Enrollment> Enrollments { get; set; } = [];
}

public class Student
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string FirstName { get; set; } = "";

    [Required, StringLength(50)]
    public string LastName { get; set; } = "";

    // Numri i indeksit (p.sh. "2024/001") - unik.
    [Required, StringLength(20)]
    public string StudentNumber { get; set; } = "";

    // I njëjti email si llogaria e Identity -> kështu e lidhim përdoruesin me studentin.
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    public List<Enrollment> Enrollments { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}";
}

// Regjistrimi = tabela lidhëse SHUMË-ME-SHUMË mes Student dhe Course,
// me një kolonë shtesë: nota. (Një student -> shumë kurse; një kurs -> shumë studentë.)
public class Enrollment
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    // null = ende pa notë. Notat në Kosovë: 5 (dështim) deri 10.
    [Range(5, 10)]
    public int? Grade { get; set; }
}
