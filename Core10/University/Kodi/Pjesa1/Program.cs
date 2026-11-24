// =====================================================================
// Program.cs - PIKA E NISJES SË APLIKACIONIT (Pjesa 1: pa autentikim)
// ---------------------------------------------------------------------
// Në .NET Core 10 nuk ka Global.asax, Startup.cs apo Web.config si në MVC5.
// Gjithçka konfigurohet këtu, në dy faza:
//   1) builder.Services...  -> REGJISTROJMË shërbimet (Dependency Injection)
//   2) app.Use.../app.Map... -> NDËRTOJMË pipeline-in (middleware) që
//                               përpunon çdo kërkesë HTTP me radhë.
// =====================================================================

using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using University.Data;
using University.Services;

// "builder" lexon automatikisht appsettings.json, appsettings.Development.json,
// User Secrets dhe variablat e mjedisit.
var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// FAZA 1: SHËRBIMET
// ---------------------------------------------------------------------

// Controller-at MVC (me View) dhe API (pa View).
builder.Services.AddControllersWithViews();

// Dokumenti OpenAPI (/openapi/v1.json) që përshkruan API-në.
builder.Services.AddOpenApi();

// DbContext i EF Core, me connection string nga appsettings.json.
builder.Services.AddDbContext<UniversityContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("UniversityDb"),
        sql => sql.EnableRetryOnFailure()));

// Kur një controller kërkon IGradeService në konstruktor, DI-ja i jep GradeService.
// Scoped = një instancë për çdo kërkesë HTTP.
builder.Services.AddScoped<IGradeService, GradeService>();

var app = builder.Build();

// ---------------------------------------------------------------------
// INICIALIZIMI I BAZËS (një herë në nisje)
// ---------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UniversityContext>();
    db.Database.Migrate();      // krijon bazën / aplikon migrimet
    SeedData.Initialize(db);    // të dhëna shembull
}

// ---------------------------------------------------------------------
// FAZA 2: PIPELINE-I (middleware) - RENDI KA RËNDËSI
// ---------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();               // /openapi/v1.json
    app.MapScalarApiReference();    // /scalar/v1
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// /Courses/Create -> CoursesController.Create()
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
