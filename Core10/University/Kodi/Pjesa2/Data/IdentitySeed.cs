using Microsoft.AspNetCore.Identity;
using University.Models;

namespace University.Data;

// Krijon ROLET dhe tre PËRDORUES shembull (një për çdo rol).
// RoleManager dhe UserManager janë shërbime të Identity që i marrim nga DI.
public static class IdentitySeed
{
    public static readonly string[] Roles = ["Admin", "Professor", "Student"];

    public static async Task InitializeAsync(IServiceProvider services, string? password)
    {
        // 1) Rolet - krijohen gjithmonë (nëse s'ekzistojnë).
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2) Përdoruesit - VETËM nëse është dhënë fjalëkalimi në konfigurim
        //    ("SeedUsers:Password"). Fjalëkalimi s'shkruhet kurrë në kod!
        if (string.IsNullOrEmpty(password))
        {
            return;
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await EnsureUserAsync(userManager, "admin@uni.test", "Administratori", "Admin", password);
        await EnsureUserAsync(userManager, "prof@uni.test", "Arben Krasniqi", "Professor", password);
        await EnsureUserAsync(userManager, "ana.gashi@student.test", "Ana Gashi", "Student", password);
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager, string email, string fullName, string role, string password)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return; // ekziston tashmë
        }

        // UserName = Email -> përdoruesi kyçet me email.
        var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
        // CreateAsync e ruan fjalëkalimin si HASH (jo tekst të lexueshëm).
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
