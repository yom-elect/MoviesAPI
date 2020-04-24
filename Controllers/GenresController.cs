using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.DTOs;
using MoviesAPI.Filters;
using MoviesAPI.Models;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController :  ControllerBase
    {
        private readonly movies_apiContext _context;
        private readonly ILogger<GenresController> _logger;
        private readonly IMapper _mapper;
        public GenresController(ILogger<GenresController> logger,
            movies_apiContext context,IMapper mapper
            )
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet] //api/genres
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var genres =  await _context.Genres.AsNoTracking().ToListAsync();
            var genresDTOs = _mapper.Map<List<GenreDTO>>(genres);
            return genresDTOs;
        }

        [HttpGet("{Id:int}", Name ="getGenre")]  //api/genres/{id}
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(GenreDTO), 200)]
        public async Task<ActionResult<GenreDTO>> Get(int Id)
        {
            
            var singleGenre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == Id);
            if (singleGenre == null)
            {
               return NotFound();
            }

            var singleGenreDTO = _mapper.Map<GenreDTO>(singleGenre);
            return singleGenreDTO;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async  Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreation)
        {
            var genre = _mapper.Map<Genre>(genreCreation);
            _context.Add(genre);
           await _context.SaveChangesAsync();
            var genreDTO = _mapper.Map<GenreDTO>(genre);
            return new CreatedAtRouteResult("getGenre", new { genreDTO.Id }, genreDTO);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id , [FromBody] GenreCreationDTO genreCreation)
        {
            var genre = _mapper.Map<Genre>(genreCreation);
            genre.Id = id;
            _context.Entry(genre).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
        /// <summary>
        /// Delete a genre
        /// </summary>
        /// <param name="id">Id of the genre to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async  Task<ActionResult> Delete(int id)
        {
            var exits = await _context.Genres.AnyAsync(x => x.Id == id);
            if (!exits)
            {
                return NotFound();
            }
           _context.Remove(new Genre() { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}