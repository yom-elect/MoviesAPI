using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Models
{
    public class Person
    {
        public int Id { get; set;}
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public string Biography { get; set; }
        public string Picture { get; set; } //byte[] to store actual picture and not path or url
        public DateTime DateOfBirth { get; set; }
        public List<MoviesActors> MoviesActors { get; set; }
    }
}
