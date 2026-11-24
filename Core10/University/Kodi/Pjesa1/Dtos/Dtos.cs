// =====================================================================
// DTO (Data Transfer Object) - "forma" e të dhënave që shkojnë/vijnë nga API.
// ---------------------------------------------------------------------
// Pse jo direkt entitetet?
//   1) Siguri: klienti s'mund të dërgojë fusha që s'duhet (p.sh. Id, Grade).
//   2) Cikle: Course -> Enrollments -> Course -> ... JSON-i do binte në lak.
//   3) Kontroll: API kthen vetëm atë që duhet (p.sh. emrin e departamentit, jo objektin).
// "record" = klasë e thjeshtë, e pandryshueshme, me konstruktor të shkurtër.
// Atributet e validimit vendosen direkt te parametrat.
// =====================================================================

using System.ComponentModel.DataAnnotations;

namespace University.Dtos;

// ---- Kurset ----
// Çfarë KTHEN API-ja (output).
public record CourseDto(int Id, string Code, string Title, int Credits, string Department, string? Professor);

// Çfarë PRANON API-ja te POST/PUT (input). [ApiController] kthen 400 automatikisht
// nëse validimi dështon (p.sh. Credits = 50).
public record CourseInputDto(
    [Required, StringLength(10)] string Code,
    [Required, StringLength(100)] string Title,
    [Range(1, 12)] int Credits,
    int DepartmentId,
    int? ProfessorId);

// ---- Studentët ----
public record StudentDto(int Id, string FullName, string StudentNumber, string Email);

// Detajet + mesatarja (e llogaritur nga IGradeService) + lista e kurseve.
public record StudentDetailsDto(
    int Id,
    string FullName,
    string StudentNumber,
    double? AverageGrade,
    IReadOnlyList<EnrollmentDto> Enrollments);

public record StudentInputDto(
    [Required, StringLength(50)] string FirstName,
    [Required, StringLength(50)] string LastName,
    [Required, StringLength(20)] string StudentNumber,
    [Required, EmailAddress] string Email);

// ---- Regjistrimet dhe notat ----
public record EnrollmentDto(int Id, string Course, int? Grade);

public record EnrollInputDto(int StudentId, int CourseId);

public record GradeInputDto([Range(5, 10)] int Grade);
