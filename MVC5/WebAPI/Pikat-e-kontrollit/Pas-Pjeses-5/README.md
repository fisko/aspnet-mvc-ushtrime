# Pika e kontrollit: pas Pjesës 5

⬅️ [Kthehu te ushtrimi](../../README.md#pjesa-6-klienti-1-faqe-me-jquery-brenda-projektit-web-api-15-min)

Projekti `UniversityApi` në gjendjen pas Pjesës 5: API i plotë me `StudentsController`, `CoursesController` dhe `EnrollmentsController`, gati për t'u përdorur nga klientët në Pjesën 6 dhe 7.

## Si ta përdorësh

1. Hap `UniversityApi.sln`.
2. Shtyp **Ctrl+Shift+B** (paketat NuGet shkarkohen vetë).
3. Te **Package Manager Console** ekzekuto `Update-Database`.
4. Nise me **Ctrl+F5** dhe kontrollo `http://localhost:5050/api/students`.
5. Vazhdo me **Pjesën 6** të ushtrimit.

> **Kujdes:** nëse ke tashmë databazën `UniversityDb` nga puna jote, `Update-Database` mund të japë gabim (p.sh. `There is already an object named 'Students'`). Fshije databazën te **SQL Server Object Explorer** (kliko me të djathtën mbi `UniversityDb` → **Delete**) dhe ekzekuto përsëri `Update-Database`.
