using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using University.Models;
using University.Services;

namespace University.Controllers.Api;

// Çfarë dërgon klienti te login: { "email": "...", "password": "..." }
public record LoginDto(string Email, string Password);

// /api/auth - "sporteli" ku klienti merr tokenin JWT.
[ApiController]
[Route("api/auth")]
public class AuthApiController(
    UserManager<ApplicationUser> userManager,       // gjen përdoruesit, rolet
    SignInManager<ApplicationUser> signInManager,   // kontrollon fjalëkalimin
    IJwtTokenService tokens) : ControllerBase       // shërbimi ynë që krijon JWT
{
    // POST /api/auth/login -> 200 { token, expiresAtUtc } ose 401
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResult>> Login(LoginDto input)
    {
        var user = await userManager.FindByEmailAsync(input.Email);
        if (user is null)
        {
            // I njëjti 401 si për fjalëkalim të gabuar -> sulmuesi s'mëson
            // nëse email-i ekziston apo jo.
            return Unauthorized();
        }

        // CheckPasswordSignInAsync: kontrollon fjalëkalimin PA krijuar cookie.
        // lockoutOnFailure: pas 5 tentimeve të gabuara llogaria bllokohet përkohësisht.
        var result = await signInManager.CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        return tokens.CreateToken(user, await userManager.GetRolesAsync(user));
    }

    // GET /api/auth/me - tregon çfarë "sheh" serveri brenda tokenit tënd.
    // Shumë e dobishme për debug: a erdhën rolet?
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Me() => Ok(new
    {
        Name = User.Identity?.Name,
        Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value)
    });
}
