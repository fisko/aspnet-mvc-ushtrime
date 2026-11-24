using Microsoft.AspNetCore.Identity;

namespace University.Models;

// Përdoruesi i aplikacionit (tabela AspNetUsers).
// IdentityUser ka tashmë: Id, UserName, Email, PasswordHash, PhoneNumber...
// Ne i shtojmë vetëm një fushë: emrin e plotë.
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
