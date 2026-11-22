# Pika e kontrollit: pas Pjesës 3

⬅️ [Kthehu te ushtrimi](../../README.md#pjesa-4-controller-at-me-crud-17-min)

Projekti `UniversityApi` në gjendjen pas Pjesës 3: modelet `Student`, `Course`, `Enrollment`, `UniversityContext`, connection string, migrimi `InitialCreate` dhe të dhënat fillestare (`Seed`). Controller-at **nuk** janë krijuar ende.

## Si ta përdorësh

1. Hap `UniversityApi.sln`.
2. Shtyp **Ctrl+Shift+B** (paketat NuGet shkarkohen vetë).
3. Te **Package Manager Console** ekzekuto `Update-Database`.
4. Vazhdo me **Pjesën 4** të ushtrimit.

> **Kujdes:** nëse ke tashmë databazën `UniversityDb` nga puna jote, `Update-Database` mund të japë gabim (p.sh. `There is already an object named 'Students'`). Fshije databazën te **SQL Server Object Explorer** (kliko me të djathtën mbi `UniversityDb` → **Delete**) dhe ekzekuto përsëri `Update-Database`.
