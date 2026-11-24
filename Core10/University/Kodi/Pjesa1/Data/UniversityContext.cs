// =====================================================================
// UniversityContext - "URA" mes kodit C# dhe bazës SQL (EF Core)
// ---------------------------------------------------------------------
// Çdo DbSet<T> = një tabelë. Me LINQ (db.Courses.Where(...)) EF Core
// gjeneron SQL-in vetë. SaveChanges() i dërgon ndryshimet në bazë.
//
// Pjesa 1: trashëgon thjesht nga DbContext.
// (Në Pjesën 2 do trashëgojë nga IdentityDbContext<ApplicationUser>.)
// =====================================================================

using Microsoft.EntityFrameworkCore;
using University.Models;

namespace University.Data;

// "Primary constructor" (C# 12): parametri options i kalohet direkt klasës bazë.
// DI-ja e krijon këtë objekt me opsionet e dhëna te AddDbContext në Program.cs.
public class UniversityContext(DbContextOptions<UniversityContext> options)
    : DbContext(options)
{
    // "=> Set<T>()" në vend të { get; set; } -> s'kemi paralajmërim për null.
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Professor> Professors => Set<Professor>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    // Fluent API: rregulla që nuk shprehen dot me atribute.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Indekse UNIKE: s'lejohen dy kurse me të njëjtin kod, etj.
        modelBuilder.Entity<Course>().HasIndex(c => c.Code).IsUnique();
        modelBuilder.Entity<Student>().HasIndex(s => s.StudentNumber).IsUnique();
        // Një student s'mund të regjistrohet dy herë në të njëjtin kurs.
        modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

        // Nëse fshihet profesori, kurset e tij mbeten - vetëm ProfessorId bëhet NULL.
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Professor)
            .WithMany(p => p.Courses)
            .OnDelete(DeleteBehavior.SetNull);

        // Restrict: s'lejohet fshirja e departamentit që ka kurse.
        // (Pa këtë SQL Server ankohet për "multiple cascade paths":
        //  Department -> Professor -> Course dhe Department -> Course.)
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Department)
            .WithMany(d => d.Courses)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
