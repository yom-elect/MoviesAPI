using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class PersonDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public string Picture { get; set; } //byte[] to store actual picture and not path or url
        public DateTime DateOfBirth { get; set; }
    }
}
