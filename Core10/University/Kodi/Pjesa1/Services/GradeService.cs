namespace University.Services;

// INTERFACE = "kontrata": çfarë di të bëjë shërbimi, pa thënë SI.
// Controller-at varen nga interface-i, jo nga klasa -> më lehtë për ta
// zëvendësuar ose testuar (shih University.Tests).
public interface IGradeService
{
    double? Average(IEnumerable<int?> grades);
}

// Implementimi konkret. Regjistrohet në Program.cs:
//   builder.Services.AddScoped<IGradeService, GradeService>();
public class GradeService : IGradeService
{
    // Mesatarja e notave, duke INJORUAR kurset pa notë (null).
    // Kthen null nëse s'ka asnjë notë; përndryshe rrumbullakon në 2 shifra.
    public double? Average(IEnumerable<int?> grades)
    {
        var graded = grades.Where(g => g.HasValue).Select(g => g!.Value).ToList();
        return graded.Count == 0 ? null : Math.Round(graded.Average(), 2);
    }
}
