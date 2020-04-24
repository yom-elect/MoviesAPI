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

namespace MoviesAPI.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly movies_apiContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly string containerName = "people";

        public PeopleController(movies_apiContext context, IMapper mapper,IFileStorageService fileStorageService)
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }
        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<List<PersonDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            var querable = _context.People.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(querable, pagination.RecordsPerPage);
            var people = await querable.Paginate(pagination).ToListAsync();
            return _mapper.Map<List<PersonDTO>>(people);
        }

        // GET: api/People/5
        [HttpGet("{id}", Name = "getPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {
            var person = await _context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            return _mapper.Map<PersonDTO>(person);
        }

        // POST: api/People
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PersonCreationDTO personCreationDTO)
        {
            var person = _mapper.Map<Person>(personCreationDTO);

            if (personCreationDTO.Picture != null)
            {
                using (var memeoryStream = new MemoryStream())
                {
                    await personCreationDTO.Picture.CopyToAsync(memeoryStream);
                    var content = memeoryStream.ToArray();
                    var extention = Path.GetExtension(personCreationDTO.Picture.FileName);     //.Split(".")[1];
                    person.Picture =
                        await _fileStorageService.SaveFile(content,
                        extention,
                        containerName,
                        personCreationDTO.Picture.ContentType);
                }
            }
           
            _context.Add(person);
            await _context.SaveChangesAsync();
            var personDTO = _mapper.Map<PersonDTO>(person);
            return new CreatedAtRouteResult("getPerson", new { id = person.Id }, personDTO);
        }

        // PUT: api/People/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PersonCreationDTO personCreationDTO)
        {
            var personDB = await _context.People.FirstOrDefaultAsync(x => x.Id == id);

            if (personDB == null)
            {
                return NotFound();
            }
            personDB = _mapper.Map(personCreationDTO, personDB);
            if (personCreationDTO.Picture != null)
            {
                using (var memeoryStream = new MemoryStream())
                {
                    await personCreationDTO.Picture.CopyToAsync(memeoryStream);
                    var content = memeoryStream.ToArray();
                    var extention = Path.GetExtension(personCreationDTO.Picture.FileName);     //.Split(".")[1];
                    personDB.Picture =
                        await _fileStorageService.EditFile(content,
                        extention,
                        containerName,
                        personDB.Picture,
                        personCreationDTO.Picture.ContentType);
                }
            }
            await _context.SaveChangesAsync();
            return NoContent();

        }
        // PATCH: api/ApiWithActions/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var entityFromDB = await _context.People.FirstOrDefaultAsync(x => x.Id == id);
            
            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = _mapper.Map<PersonPatchDTO>(entityFromDB);
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

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exits = await _context.People.AnyAsync(x => x.Id == id);
            if (!exits)
            {
                return NotFound();
            }
            _context.Remove(new Person() { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
