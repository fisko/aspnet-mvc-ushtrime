# Kodi i ushtrimit

Ky folder përmban klasat e ushtrimit, që t'i kopjosh kur ngec ose kur do të krahasosh kodin tënd. Krijo projektet vetë sipas ushtrimit, pastaj kopjo skedarët në po të njëjtat foldera.

| Skedari | Ku vendoset | Pjesa |
| --- | --- | --- |
| `UniversityApi/App_Start/WebApiConfig.cs` | Zëvendëson metodën `Register` | 1 |
| `UniversityApi/Models/*.cs` | Folderi `Models` | 2, 5 |
| `UniversityApi/Web.config.connectionStrings.xml` | Bllok për brenda `Web.config` | 2 |
| `UniversityApi/Migrations/Configuration.cs` | Pas `Enable-Migrations` | 3 |
| `UniversityApi/Controllers/*.cs` | Folderi `Controllers` | 4, 5 |
| `UniversityApi/Controllers/HomeController.Students.cs` | Metodë për `HomeController` | 6 |
| `UniversityApi/Views/Home/Students.cshtml` | Folderi `Views/Home` | 6 |
| `UniversityMvcClient/**` | Projekti MVC klient | 7 |

Kujdes: `Update-Database` duhet ekzekutuar vetë pas `Add-Migration InitialCreate`. Controller-i `CoursesController` dhe view-t e MVC krijohen me scaffolding.