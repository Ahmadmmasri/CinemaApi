using AuthenticationPlugin;
using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        private CinemaDbContext _dbContext;

        public UsersController(CinemaDbContext dbContext, IConfiguration configuration)
        {
            this._configuration = configuration;
            this._auth = new AuthService(_configuration);
            this._dbContext = dbContext;

        }

        [HttpPost]
        public IActionResult Register([FromBody] User user) 
        {
           var IsUserExcsite= _dbContext.Users.Where(x => x.Email == user.Email).SingleOrDefault();
            if (IsUserExcsite != null)
            {
                return BadRequest("User with same email is exists");
            }

            var userObj = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password= SecurePasswordHasherHelper.Hash(user.Password),
                Role= "Users"
            };
            _dbContext.Users.Add(userObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        public IActionResult Login([FromBody] User user)
        {
            var UserEmail = _dbContext.Users.FirstOrDefault(x => x.Email == user.Email);
            if(UserEmail == null)
            {
                return NotFound();
            }
            if( ! SecurePasswordHasherHelper.Verify(user.Password, UserEmail.Password) )
            {
                return Unauthorized();
            }

            var claims = new[]
             {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                //for get role in token
                new Claim(ClaimTypes.Role, UserEmail.Role),
            };

            var token = _auth.GenerateAccessToken(claims);

            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                user_Id= UserEmail.Id,
            });

        }

        [Authorize(Roles ="Admin")]
        [HttpGet("{id}")]
        public string TestAdmin(int id)
        {
            return id+" Hello Admin";
        }

        [Authorize(Roles = "Users")]
        [HttpGet("{id}")]
        public string TestUser(int id)
        {
            return id+" Hello User";
        }
    }
}
