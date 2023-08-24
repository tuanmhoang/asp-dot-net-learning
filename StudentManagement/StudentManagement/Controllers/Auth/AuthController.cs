using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Helpers;
using StudentManagement.Models.Entities;
using StudentManagement.Models.Requests;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace StudentManagement.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class AuthController : ControllerBase
    {
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
            string pattern = @"\w{4}";
            bool isMatch = Regex.IsMatch(request.Username, pattern);
            if (!isMatch)
            {
                return BadRequest("Username can only have character and number, at least 4 characters length");
            }

            // check password regex

            // create salt
            PasswordHelper passwordHelper = new PasswordHelper();
            int saltSize = 16; 
            byte[] salt = passwordHelper.GenerateSalt(saltSize);

            // create password
            CreatePasswordHash(
                request.Password,
                out byte[] passwordHash);

            // create user
            User user = new User();
            user.Username = request.Username;
            user.Password = Encoding.UTF8.GetString(passwordHash); 
            user.PasswordSalt = salt;

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
            var slt = foundUser.PasswordSalt;

            if (!VerifyPasswordHash(
                    request.Password,
                    Encoding.UTF8.GetBytes(foundUser.Password),
                    foundUser.PasswordSalt
                    )
                )
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(foundUser);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
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
