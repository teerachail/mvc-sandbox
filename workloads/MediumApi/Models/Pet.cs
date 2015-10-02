using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediumApi.Models
{
    public class Pet
    {
        public int Id { get; set; }

        public Category Category { get; set; }

        [Required]
        public string Name { get; set; }

        public List<string> Urls { get; set; }

        public List<Tag> Tags { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
