using System.ComponentModel.DataAnnotations;

namespace University.Models;

// ViewModel = klasë që i përshtatet një FORME (jo një tabele).
// Nuk e ruajmë në bazë; vetëm mbart të dhënat nga View -> Controller.

// Forma e hyrjes (/Account/Login).
public class LoginViewModel
{
    [Required(ErrorMessage = "Email-i është i detyrueshëm.")]
    [EmailAddress(ErrorMessage = "Email-i nuk është valid.")]
    public string Email { get; set; } = "";

    // DataType.Password -> Razor e shfaq si <input type="password">.
    [Required(ErrorMessage = "Fjalëkalimi është i detyrueshëm.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Më mbaj të kyçur")]
    public bool RememberMe { get; set; }
}

// Forma e regjistrimit (/Account/Register).
public class RegisterViewModel
{
    [Required(ErrorMessage = "Emri është i detyrueshëm.")]
    [Display(Name = "Emri i plotë")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Email-i është i detyrueshëm.")]
    [EmailAddress(ErrorMessage = "Email-i nuk është valid.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Fjalëkalimi është i detyrueshëm.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    // [Compare] kontrollon që të dy fushat të jenë të njëjta.
    [DataType(DataType.Password)]
    [Display(Name = "Konfirmo fjalëkalimin")]
    [Compare(nameof(Password), ErrorMessage = "Fjalëkalimet nuk përputhen.")]
    public string ConfirmPassword { get; set; } = "";
}
