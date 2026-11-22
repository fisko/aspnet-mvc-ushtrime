# Ushtrime ASP.NET MVC

Ushtrime hap pas hapi për studentët, nga **ASP.NET MVC 5** (.NET Framework) deri te **ASP.NET Core 10**.

Fakulteti i Shkencave të Informatikës, Universiteti "Nënë Tereza", Shkup · Doc. Dr. Fisnik Doko

## Si t'i përdorësh

1. Kliko **Code → Download ZIP** dhe hape ZIP-in, ose klono repon:

   ```
   git clone https://github.com/fisko/aspnet-mvc-ushtrime.git
   ```

2. Hap folderin e ushtrimit dhe ndiq `README.md` brenda tij. Çdo ushtrim ka teori, vizatime, hapat e projektit, pika kontrolli dhe pyetje kontrolli.
3. Projektin e ndërton vetë në Visual Studio, hap pas hapi. Zgjidhjet e plota nuk publikohen; te disa ushtrime ka **pika kontrolli** që të lejojnë të vazhdosh nëse ngec.

## Ushtrimet

| Ushtrimi | Tema | Teknologjitë |
| --- | --- | --- |
| [WebAPI](MVC5/WebAPI) | Web API për studentë dhe kurse, me dy klientë (jQuery dhe MVC) | MVC 5, Web API 2, EF6, LocalDB, Postman |

Ushtrimet e tjera shtohen gjatë semestrit.

## Kërkesat për MVC 5

- Visual Studio 2022 me workload-et **ASP.NET and web development** dhe **Data storage and processing**
- .NET Framework 4.8 dhe SQL Server LocalDB (instalohen me Visual Studio)
- Postman, për ushtrimet me Web API

## Struktura

```
aspnet-mvc-ushtrime/
├── MVC5/
│   └── WebAPI/
│       ├── README.md              ← ushtrimi
│       ├── Kodi/                  ← klasat e ushtrimit
│       └── img/                   ← vizatimet
└── Core10/                        ← ushtrimet me ASP.NET Core 10
```

