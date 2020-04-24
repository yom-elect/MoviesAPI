using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Models
{
    public class MoviesGenres
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public Movie Movie { get; set; }
        public Genre Genre { get; set; }
    }
}
