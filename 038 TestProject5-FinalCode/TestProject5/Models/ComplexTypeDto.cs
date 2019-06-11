using System;
using System.ComponentModel.DataAnnotations;

namespace TestProject5.Models
{
    public class ComplexTypeDto
    {
        [Required]
        [MaxLength(12)]
        public string String1 { get; set; }
        [RegularExpression(@"^[A-Za-z0-9]{4,12}$")]
        public string String2 { get; set; }
        [Range(0,100)]
        public int Int1 { get; set; }
        public int Int2 { get; set; }
        public DateTime? Date1 { get; set; }
    }
}