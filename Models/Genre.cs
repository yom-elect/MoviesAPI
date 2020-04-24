using MoviesAPI.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Models
{
    public partial class Genre
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        [FirstLetterUppercase]
        public string Name { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
    }
}
