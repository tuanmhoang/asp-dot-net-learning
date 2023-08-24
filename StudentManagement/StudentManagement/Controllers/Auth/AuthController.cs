using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Models;
using StudentManagement.Models.Entities;
using StudentManagement.Models.Requests;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudentManagement.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration configuration;
        private readonly StudentManagementContext dbContext;

        public AuthController(IConfiguration configuration, StudentManagementContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        [SwaggerOperation(Summary = "Register a user")]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto request)
        {
            // check username regex

            // check password regex

            // create password
            CreatePasswordHash(
                request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            user.Username = request.Username;
            user.Password = Encoding.UTF8.GetString(passwordHash); 
            user.PasswordSalt = Encoding.UTF8.GetString(passwordSalt);

            // save to db
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Ok(user);
        }

        [SwaggerOperation(Summary = "Login")]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            var foundUser = dbContext.Users.FirstOrDefault(u => u.Username == request.Username);
            if (foundUser == null)
            {
                return BadRequest("User not found.");
            }

            var pwd = Encoding.UTF8.GetBytes(foundUser.Password);
            var slt = Encoding.UTF8.GetBytes(foundUser.PasswordSalt);

            if (!VerifyPasswordHash(
                    request.Password,
                    Encoding.UTF8.GetBytes(foundUser.Password),
                    Encoding.UTF8.GetBytes(foundUser.PasswordSalt)
                    )
                )
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
