// =====================================================================
// TESTET (xUnit) - kod që teston kodin tonë automatikisht.
// ---------------------------------------------------------------------
// [Fact] = një test. Struktura "AAA":
//   Arrange - përgatit të dhënat
//   Act     - thirr metodën që testohet
//   Assert  - kontrollo rezultatin (nëse s'përputhet -> testi DËSHTON)
// Ekzekutimi: Test Explorer në Visual Studio, ose "dotnet test".
// Testojmë shërbimet (pa bazë, pa HTTP) -> testet janë të shpejta.
// =====================================================================

using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using University.Models;
using University.Services;

namespace University.Tests;

public class GradeServiceTests
{
    // Shërbimi s'ka varësi -> e krijojmë direkt me "new" (pa DI).
    private readonly GradeService _service = new();

    [Fact]
    public void Average_IgnoresUngradedEnrollments()
    {
        // Act: 10, 9 dhe një kurs pa notë (null)
        var result = _service.Average([10, 9, null]);

        // Assert: null injorohet -> (10 + 9) / 2 = 9.5
        Assert.Equal(9.5, result);
    }

    [Fact]
    public void Average_ReturnsNull_WhenNothingIsGraded()
    {
        // Asnjë notë -> s'ka mesatare (jo 0!)
        Assert.Null(_service.Average([null, null]));
    }

    [Fact]
    public void Average_RoundsToTwoDecimals()
    {
        // 25 / 3 = 8.3333... -> 8.33
        Assert.Equal(8.33, _service.Average([8, 8, 9]));
    }
}

public class JwtTokenServiceTests
{
    // Options.Create(...) zëvendëson atë që në aplikacion e bën DI + appsettings.
    private static JwtTokenService CreateService() => new(Options.Create(new JwtOptions
    {
        Key = "celes-testi-shume-e-gjate-min-32-karaktere!!",
        ExpiresMinutes = 30
    }));

    [Fact]
    public void CreateToken_ContainsEmailAndRoles()
    {
        // Arrange
        var user = new ApplicationUser { Id = "u1", UserName = "admin@uni.test", Email = "admin@uni.test" };

        // Act: krijo tokenin dhe "lexoje" (dekodoje) sërish
        var result = CreateService().CreateToken(user, ["Admin", "Professor"]);
        var token = new JsonWebTokenHandler().ReadJsonWebToken(result.Token);

        // Assert: tokeni mban email-in dhe të dy rolet
        Assert.Equal("admin@uni.test", token.GetClaim(JwtRegisteredClaimNames.Email).Value);
        var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        Assert.Equal(["Admin", "Professor"], roles);
    }

    [Fact]
    public void CreateToken_ExpiresInConfiguredTime()
    {
        var result = CreateService().CreateToken(new ApplicationUser { Id = "u1" }, []);

        // Skadimi duhet të jetë ~30 minuta nga tani (me pak tolerancë)
        Assert.InRange(result.ExpiresAtUtc, DateTime.UtcNow.AddMinutes(29), DateTime.UtcNow.AddMinutes(31));
    }
}
