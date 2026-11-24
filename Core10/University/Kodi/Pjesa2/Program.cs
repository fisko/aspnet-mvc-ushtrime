// =====================================================================
// Program.cs - PIKA E NISJES SË APLIKACIONIT (Pjesa 2: me Identity + JWT)
// ---------------------------------------------------------------------
// Në .NET Core 10 nuk ka Global.asax, Startup.cs apo Web.config si në MVC5.
// Gjithçka konfigurohet këtu, në dy faza:
//   1) builder.Services...  -> REGJISTROJMË shërbimet (Dependency Injection)
//   2) app.Use.../app.Map... -> NDËRTOJMË pipeline-in (middleware) që
//                               përpunon çdo kërkesë HTTP me radhë.
// Ky skedar përdor "top-level statements": nuk ka klasë Program as
// metodë Main - kompajleri i krijon vetë.
// =====================================================================

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using University.Data;
using University.Models;
using University.Services;

// "builder" lexon automatikisht konfigurimin nga:
// appsettings.json -> appsettings.{Environment}.json -> User Secrets (vetëm Development)
// -> variablat e mjedisit (p.sh. App Settings në Azure). I fundit e mbishkruan të parin.
var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// FAZA 1: SHËRBIMET (Dependency Injection container)
// ---------------------------------------------------------------------

// Aktivizon Controller-at MVC (me View) dhe API (pa View).
builder.Services.AddControllersWithViews();

// Gjeneron dokumentin OpenAPI (/openapi/v1.json) që përshkruan API-në.
builder.Services.AddOpenApi();

// Regjistron DbContext-in e EF Core. Connection string-u merret nga
// "ConnectionStrings:UniversityDb" në appsettings.json (ose nga Azure).
// EnableRetryOnFailure: provon sërish nëse lidhja me SQL dështon përkohësisht
// (ndodh shpesh me Azure SQL Serverless kur "zgjohet").
builder.Services.AddDbContext<UniversityContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("UniversityDb"),
        sql => sql.EnableRetryOnFailure()));

// Regjistrojmë shërbimin tonë: kur dikush kërkon IGradeService,
// DI-ja i jep një GradeService. "Scoped" = një instancë për çdo kërkesë HTTP.
builder.Services.AddScoped<IGradeService, GradeService>();

// ---- ASP.NET Core Identity (përdoruesit, fjalëkalimet, rolet) --------
// Identity ruan përdoruesit në tabelat AspNetUsers, AspNetRoles... brenda
// të njëjtës bazë (UniversityContext). Për faqet MVC përdor COOKIE.
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;           // fjalëkalim min. 8 karaktere
        options.Lockout.MaxFailedAccessAttempts = 5;   // pas 5 tentimeve të gabuara bllokohet
    })
    .AddEntityFrameworkStores<UniversityContext>()
    .AddDefaultTokenProviders();

// Ku ridrejtohet përdoruesi kur nuk është i kyçur / nuk ka leje.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// ---- JWT (për Web API) ----------------------------------------------
// Seksioni "Jwt" nga konfigurimi lidhet me klasën JwtOptions (Options pattern).
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
// Çelësi sekret NUK vendoset në appsettings.json (do përfundonte në GitHub!).
// Lokalisht: User Secrets. Në Azure: App Setting "Jwt__Key".
if (jwt.Key.Length < 32)
{
    throw new InvalidOperationException("Jwt:Key mungon ose është shumë i shkurtër (min. 32 karaktere).");
}

// AddIdentity më lart e bëri COOKIE skemën kryesore (default).
// Këtu SHTOJMË skemën JWT Bearer, por jo si default - prandaj në
// controller-at API shkruajmë [Authorize(AuthenticationSchemes = "Bearer")].
builder.Services
    .AddAuthentication()
    .AddJwtBearer(options =>
    {
        // Çfarë kontrollohet te çdo token që vjen në header "Authorization: Bearer ...".
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,               // kush e lëshoi tokenin?
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,             // për kë është tokeni?
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,     // a është nënshkruar me çelësin tonë?
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateLifetime = true,             // a ka skaduar?
            ClockSkew = TimeSpan.FromMinutes(1)  // toleranca e orës (default është 5 min)
        };
    });

// Policy = rregull autorizimi me emër. "GradeEditors" lejon Admin OSE Professor.
// Përdoret me [Authorize(Policy = "GradeEditors")].
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("GradeEditors", policy => policy.RequireRole("Admin", "Professor"));

// Nga këtu e tutje lista e shërbimeve "ngrihet" dhe nuk mund të ndryshohet më.
var app = builder.Build();

// ---------------------------------------------------------------------
// INICIALIZIMI I BAZËS (një herë në nisje)
// ---------------------------------------------------------------------
// DbContext është "Scoped", prandaj na duhet një scope i përkohshëm.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UniversityContext>();
    db.Database.Migrate();      // krijon bazën / aplikon migrimet që mungojnë
    SeedData.Initialize(db);    // fakultete, kurse, studentë shembull
    // Rolet + përdoruesit shembull. Fjalëkalimi vjen nga konfigurimi (jo i shkruar në kod).
    await IdentitySeed.InitializeAsync(scope.ServiceProvider, builder.Configuration["SeedUsers:Password"]);
}

// ---------------------------------------------------------------------
// FAZA 2: PIPELINE-I (middleware). RENDI KA RËNDËSI!
// Çdo kërkesë kalon nëpër këto hapa nga lart poshtë.
// ---------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();               // /openapi/v1.json
    app.MapScalarApiReference();    // /scalar/v1 - UI për të testuar API-në
}
else
{
    app.UseExceptionHandler("/Home/Error"); // faqe gabimi miqësore në prodhim
    app.UseHsts();                          // shfletuesi detyrohet të përdorë HTTPS
}

app.UseHttpsRedirection();  // http:// -> https://
app.UseRouting();           // gjen cilin controller/action e përputh URL-ja

app.UseAuthentication();    // KUSH je? (lexon cookie-n ose tokenin JWT)
app.UseAuthorization();     // A KE LEJE? (kontrollon [Authorize], rolet, policy)

app.MapStaticAssets();      // CSS, JS, imazhet nga wwwroot/

// Rruga standarde MVC: /Courses/Create -> CoursesController.Create()
// Controller-at API kanë rrugët e tyre me [Route("api/...")].
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run(); // nis serverin Kestrel dhe pret kërkesa
