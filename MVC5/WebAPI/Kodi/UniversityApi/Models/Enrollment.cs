using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace UniversityApi.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        [Range(5, 10)]
        public int? Grade { get; set; }

        [JsonIgnore]
        public Student Student { get; set; }

        [JsonIgnore]
        public Course Course { get; set; }
    }
}
