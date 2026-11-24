using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using University.Models;

namespace University.Controllers;

// Faqet e llogarisë (MVC, me COOKIE): Login, Register, Logout, AccessDenied.
// Krahasoni me AuthApiController që kthen JWT për API-në.
public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : Controller
{
    // GET /Account/Login?returnUrl=/Students
    // returnUrl = faqja ku donte të shkonte përdoruesi para se ta ridrejtonim këtu.
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // mbrojtje nga CSRF - forma duhet të vijë nga faqja jonë
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model); // shfaq sërish formën me gabimet
        }

        // PasswordSignInAsync: kontrollon fjalëkalimin DHE krijon cookie-n e autentikimit.
        var result = await signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            // Url.IsLocalUrl: mos ridrejto kurrë te një faqe e jashtme (open redirect).
            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
        }

        // "" = gabim për gjithë formën (shfaqet te asp-validation-summary).
        ModelState.AddModelError("", result.IsLockedOut ? "Llogaria është e bllokuar përkohësisht." : "Email ose fjalëkalim i pasaktë.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
        // Identity kontrollon rregullat e fjalëkalimit (gjatësia, shifra, shkronja e madhe...).
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // Kushdo që regjistrohet vetë merr rolin më të ulët: Student.
        // Rolet Admin/Professor jepen vetëm nga administratori (këtu: nga IdentitySeed).
        await userManager.AddToRoleAsync(user, "Student");
        await signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    // Logout është POST (jo GET) që një link/imazh i huaj të mos të nxjerrë jashtë.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync(); // fshin cookie-n
        return RedirectToAction("Index", "Home");
    }

    // Këtu vjen përdoruesi i kyçur që s'ka rolin e duhur (403).
    public IActionResult AccessDenied() => View();
}
