using System.ComponentModel.DataAnnotations;

namespace UniversityMvcClient.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Emri është i detyrueshëm")]
        [StringLength(50)]
        [Display(Name = "Emri")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Mbiemri është i detyrueshëm")]
        [StringLength(50)]
        [Display(Name = "Mbiemri")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email-i është i detyrueshëm")]
        [EmailAddress(ErrorMessage = "Email-i nuk është i vlefshëm")]
        [StringLength(100)]
        public string Email { get; set; }
    }
}
