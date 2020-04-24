using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.DTOs;
using MoviesAPI.DTOs.UserAccount;
using MoviesAPI.Helpers;
using MoviesAPI.Models;

namespace MoviesAPI.Controllers
{
    [Route("api/auths")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly movies_apiContext _context;
        private readonly IMapper _mapper;

        public AccountsController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            movies_apiContext context,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
          _context = context;
            _mapper = mapper;
        }

        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(UserToken),200)]
        [HttpPost("Create")]
        public async Task<ActionResult<UserToken>> CreateUser ([FromBody] UserInfo model)
        {
            var user = new IdentityUser { UserName = model.EmailAddress ,Email = model.EmailAddress };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return await BuildToken(model);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> LoginUser ([FromBody] UserInfo model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password,
                isPersistent: false , lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(model);
            }
            else
            {
                return BadRequest("Invalid Login Attempt");
            }
        }

        [HttpPost("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> RenewUserToken()
        {
            var userInfo = new UserInfo {
                EmailAddress = HttpContext.User.Identity.Name
            };
            return  await BuildToken(userInfo);
        }

        private async Task<UserToken> BuildToken(UserInfo userInfo)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userInfo.EmailAddress),
                new Claim(ClaimTypes.Email, userInfo.EmailAddress),
                new Claim("mykey", "Whatever value i choose"),
            };

            var identityUser = await _userManager.FindByEmailAsync(userInfo.EmailAddress);
            var claimsDB = await _userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsDB);

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(30);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims : claims,
                expires: expiration,
                signingCredentials:creds
                );

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserDTO>>>  GetUsers ([FromQuery] PaginationDTO paginationDTO)
        {
            var querable = _context.Users.AsQueryable();
            querable = querable.OrderBy(x => x.Email);
            await HttpContext.InsertPaginationParametersInResponse(querable, paginationDTO.RecordsPerPage);
            var users = await querable.Paginate(paginationDTO).ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        [HttpGet("roles")]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            return await _context.Roles.Select(x => x.Name).ToListAsync();
        }

        [HttpPost("AssignRole")]
        public async Task<ActionResult> AssignRole(EditRoleDTO editRoleDTO)
        {
            var user = await _userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
            return NoContent();
        }

        [HttpPost("RemoveRole")]
        public async Task<ActionResult> RemoveRole(EditRoleDTO editRoleDTO)
        {
            var user = await _userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
            return NoContent();
        } 
    }
}