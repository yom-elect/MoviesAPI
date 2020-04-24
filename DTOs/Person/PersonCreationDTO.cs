using Microsoft.AspNetCore.Http;
using MoviesAPI.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class PersonCreationDTO
    {
        [Required]
        [StringLength(120)] 
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Picture { get; set; }
    }
}
