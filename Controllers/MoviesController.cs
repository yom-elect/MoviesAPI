using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Helpers;
using MoviesAPI.Models;
using MoviesAPI.Services;
using System.Linq.Dynamic.Core;

namespace MoviesAPI.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly movies_apiContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly string containerName = "movies";

        public MoviesController(movies_apiContext context,IMapper mapper,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IndexMoviePageDTO>> GetMovies()
        {
            var top = 6;
            var today = DateTime.Today;
            var upcomingReleases = await _context.Movies
                    .Where(x => x.ReleaseDate > today)
                    .OrderBy(x => x.ReleaseDate)
                    .Take(top)
                    .ToListAsync();

            var inTheaters = await _context.Movies
                   .Where(x => x.InTheaters)
                   .Take(top)
                   .ToListAsync();

            var result = new IndexMoviePageDTO();
            result.InThearters = _mapper.Map<List<MovieDTO>>(inTheaters);
            result.UpcomingReleases = _mapper.Map<List<MovieDTO>>(upcomingReleases);

            //var movies = await _context.Movies.ToListAsync();
            return result;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> GetFilteredMovies([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }
            if (filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }
            if (filterMoviesDTO.UpComingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }
            if (filterMoviesDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenres.Select(y => y.GenreId)
                    .Contains(filterMoviesDTO.GenreId));
            }

            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.OrderingField))
            {
                try
                {
                    moviesQueryable = moviesQueryable
                  .OrderBy($"{filterMoviesDTO.OrderingField} {(filterMoviesDTO.AscendingOrder ? "ascending" : "descending")}");
                }
                catch
                {
                    //logger 
                }
              
            }

            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable, filterMoviesDTO.RecordsPerPage);

            var movies = await moviesQueryable.Paginate(filterMoviesDTO.Pagination).ToListAsync();
            return _mapper.Map<List<MovieDTO>>(movies);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDetailsDTO>> GetMovie(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x=>x.Person)
                .Include(x=>x.MoviesGenres).ThenInclude(x=>x.Genre)
                .FirstOrDefaultAsync(x=>x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieDetailsDTO>(movie);
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movieDB = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movieDB == null)
            {
                return NotFound();
            }
            movieDB = _mapper.Map(movieCreationDTO, movieDB);

            if (movieCreationDTO.Poster != null)
            {
                using (var memeoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memeoryStream);
                    var content = memeoryStream.ToArray();
                    var extention = Path.GetExtension(movieCreationDTO.Poster.FileName);     //.Split(".")[1];
                    movieDB.Poster =
                        await _fileStorageService.SaveFile(content,
                        extention,
                        containerName,
                        movieCreationDTO.Poster.ContentType);
                }
            }

            await _context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movieDB.Id}; delete from MoviesGenres where MovieId = {movieDB.Id}");
            AnnotateActorsOrder(movieDB);
            await _context.SaveChangesAsync();
            return NoContent();

        }

        // POST: api/Movies
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult> PostMovie([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = _mapper.Map<Movie>(movieCreationDTO);

            if (movieCreationDTO.Poster != null)
            {
                using (var memeoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memeoryStream);
                    var content = memeoryStream.ToArray();
                    var extention = Path.GetExtension(movieCreationDTO.Poster.FileName);     //.Split(".")[1];
                    movie.Poster =
                        await _fileStorageService.SaveFile(content,
                        extention,
                        containerName,
                        movieCreationDTO.Poster.ContentType);
                }
            }
            AnnotateActorsOrder(movie);
            _context.Add(movie);
            await _context.SaveChangesAsync();
            var movieDTO = _mapper.Map<MovieDTO>(movie);
            return CreatedAtAction("GetMovie", new { id = movie.Id }, movieDTO);
        }

        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                } 
            }
        }

        // PATCH: api/ApiWithActions/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var entityFromDB = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = _mapper.Map<MoviePatchDTO>(entityFromDB);
            patchDocument.ApplyTo(entityDTO, ModelState);
            var isValid = TryValidateModel(entityDTO);

            if (!isValid)
            {
                return BadRequest();
            }
            _mapper.Map(entityDTO, entityFromDB);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id)
        {
            //var movie = await _context.Movies.FindAsync(id);
            var exists = await _context.Movies.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Movie() { Id = id});
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
