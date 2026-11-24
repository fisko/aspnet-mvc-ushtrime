# Ushtrimi gjithëpërfshirës: "Universiteti" me ASP.NET Core 10

Ndërtojmë një aplikacion për një fakultet: **departamente, profesorë, kurse, studentë dhe regjistrime me nota**. Ushtrimi bashkon gjithçka nga MVC 5 (MVC, Web API, EF), por në **.NET 10**, dhe shton sigurinë dhe publikimin në cloud.

| Pjesa | Tema | Koha |
|---|---|---|
| **[Pjesa 1](Pjesa1.md)** | Struktura e .NET 10, EF Core 10 (Code First, migrime, seed), MVC me Tag Helpers, Web API me DTO, ProblemDetails, Scalar dhe `.http` | ~2 orë |
| **[Pjesa 2](Pjesa2.md)** | ASP.NET Core Identity (cookie), role dhe policy, JWT për API, IDOR, testet xUnit, publikimi në Azure (App Service + Azure SQL) | ~2.5 orë |

Çdo pjesë fillon me **teori dhe vizatime**, pastaj vijnë **hapat**, gabimet e shpeshta dhe pyetjet kontrolluese.

## Kërkesat

- **Visual Studio 2026** me workload-in *ASP.NET and web development* (ose VS Code + C# Dev Kit)
- **.NET 10 SDK** (`dotnet --version` → `10.0.x`)
- **SQL Server LocalDB** (vjen me Visual Studio)
- Për Pjesën 2: llogari **Azure for Students**

## Dosja `Kodi/`

| Dosja | Përmbajtja |
|---|---|
| [`Kodi/Pjesa1`](Kodi/Pjesa1) | të gjitha klasat dhe view-të e Pjesës 1, me komente në shqip |
| [`Kodi/Pjesa2`](Kodi/Pjesa2) | **vetëm** skedarët e rinj ose të ndryshuar në Pjesën 2, plus testet |

Projektin e krijoni vetë (`University`), pastaj kopjoni skedarët sipas hapave. **Lexojini komentet**: aty shpjegohet çfarë bën çdo rresht i rëndësishëm.

## Përdoruesit shembull (Pjesa 2)

| Email | Roli | Fjalëkalimi (lokalisht) |
|---|---|---|
| `admin@uni.test` | Admin | `Parola123!` |
| `prof@uni.test` | Professor | `Parola123!` |
| `ana.gashi@student.test` | Student | `Parola123!` |

⬅️ [Kthehu te ushtrimet ASP.NET Core 10](../README.md)
