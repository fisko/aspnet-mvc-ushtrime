using System.ComponentModel.DataAnnotations;

namespace UniversityMvcClient.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Titulli është i detyrueshëm")]
        [StringLength(100)]
        [Display(Name = "Titulli")]
        public string Title { get; set; }

        [Range(1, 30, ErrorMessage = "Kreditet duhet të jenë mes 1 dhe 30")]
        [Display(Name = "Kredite")]
        public int Credits { get; set; }
    }
}
