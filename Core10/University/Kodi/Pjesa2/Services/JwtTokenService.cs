// =====================================================================
// JwtTokenService - krijon "biletën" (JWT) që klienti e dërgon te API.
// ---------------------------------------------------------------------
// Një JWT ka 3 pjesë të ndara me pikë:  HEADER.PAYLOAD.SIGNATURE
//   - HEADER:    algoritmi (HS256)
//   - PAYLOAD:   "claims" - kush është përdoruesi, rolet, skadimi...
//   - SIGNATURE: nënshkrimi me çelësin sekret -> askush s'mund ta ndryshojë
//                tokenin pa e prishur nënshkrimin.
// KUJDES: payload-i është vetëm Base64, NUK është i enkriptuar.
// Provojeni në https://jwt.io - mos vendosni kurrë fjalëkalime në token!
// =====================================================================

using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using University.Models;

namespace University.Services;

// Lidhet me seksionin "Jwt" të konfigurimit (Options pattern).
// Vlerat default këtu; Key DUHET të vijë nga User Secrets / Azure.
public class JwtOptions
{
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "University";
    public string Audience { get; set; } = "UniversityClients";
    public int ExpiresMinutes { get; set; } = 60;
}

// Çfarë i kthejmë klientit pas login-it: { "token": "...", "expiresAtUtc": "..." }
public record TokenResult(string Token, DateTime ExpiresAtUtc);

public interface IJwtTokenService
{
    TokenResult CreateToken(ApplicationUser user, IEnumerable<string> roles);
}

// IOptions<JwtOptions> e injekton DI-ja (falë Configure<JwtOptions> në Program.cs).
public class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public TokenResult CreateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        // CLAIMS = pohime për përdoruesin që shkojnë brenda tokenit.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),          // "subject" - Id e përdoruesit
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.Name, user.UserName ?? "")           // -> User.Identity.Name në controller
        };
        // Një claim për çdo rol -> [Authorize(Roles = "Admin")] i lexon këto.
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var expires = DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,       // duhet të përputhet me ValidIssuer në Program.cs
            Audience = _options.Audience,   // duhet të përputhet me ValidAudience
            Expires = expires,
            // HMAC-SHA256: i njëjti çelës nënshkruan (këtu) dhe verifikon (JwtBearer).
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new TokenResult(new JsonWebTokenHandler().CreateToken(descriptor), expires);
    }
}
