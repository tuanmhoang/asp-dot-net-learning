using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Converter;
using StudentManagement.Helpers;
using StudentManagement.Models.Dtos;
using StudentManagement.Models.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentManagement.Controllers.Setup
{
    [Route("api/[controller]")]
    [ApiController]
    public class DbController : ControllerBase
    {
        private readonly StudentManagementContext dbContext;

        private PasswordHelper passwordHelper;

        public DbController(StudentManagementContext dbContext)
        {
            this.dbContext = dbContext;
            passwordHelper = new PasswordHelper();
        }

        [SwaggerOperation(Summary = "Init db")]
        [HttpGet("init")]

        public async Task<ActionResult<string>> CreateDatabase()
        {
            string databasename = dbContext.Database.GetDbConnection().Database;// mydata
            bool result = await dbContext.Database.EnsureCreatedAsync();
            string resultstring = result ? "- Created db" : "- DB exists. Skip creating";
            string createdResult = databasename + resultstring;
    
            return Ok(createdResult);
        }

        [SwaggerOperation(Summary = "Delete db")]
        [HttpGet("delete")]
        public async Task<ActionResult<string>> DeleteDatabase()
        {
            bool deleted = await dbContext.Database.EnsureDeletedAsync();
            string deletionInfo = deleted ? "Deleted" : "Cannot delete";

            return Ok(deletionInfo);
        }

        [SwaggerOperation(Summary = "Init mock data")]
        [HttpGet("mock")]
        public async Task<ActionResult<string>> CreateMockData()
        {

            await CreateMockRoles();
            await CreateMockStudents();

            await dbContext.SaveChangesAsync();
            return Ok("Done creating mock data");
        }

        private async Task CreateMockStudents()
        {
            List<User> users = new List<User>
            {
            createSingleStudent("messi", "Lionel", "Messi"),
            createSingleStudent("ronaldo", "Cristiano", "Ronaldo"),
            createSingleStudent("neymar", "Neymar", "da Silva Santos"),
            createSingleStudent("mbappe", "Kylian", "Mbappe"),
            createSingleStudent("haaland", "Erling", "Haaland"),
            createSingleStudent("debruyne", "Kevin", "De Bruyne"),
            createSingleStudent("lewandowski", "Robert", "Lewandowski"),
            createSingleStudent("salah", "Mohamed", "Salah"),
            createSingleStudent("modric", "Luka", "Modric"),
            createSingleStudent("kane", "Harry", "Kane"),
            createSingleStudent("griezmann", "Antoine", "Griezmann"),
            createSingleStudent("pogba", "Paul", "Pogba"),
            createSingleStudent("vardy", "Jamie", "Vardy"),
            createSingleStudent("alves", "Dani", "Alves"),
            createSingleStudent("silva", "Bernardo", "Silva"),
            createSingleStudent("james", "James", "Rodriguez"),
            createSingleStudent("sterling", "Raheem", "Sterling"),
            createSingleStudent("aguero", "Sergio", "Aguero"),
            createSingleStudent("thiago", "Thiago", "Alcantara"),
            createSingleStudent("hazard", "Eden", "Hazard")
            };

            // save to db
            await dbContext.Users.AddRangeAsync(users);

        }


        private async Task CreateMockRoles()
        {
            List<Role> roles = new List<Role>
            {
                createSingleRole( "admin"),
                createSingleRole( "student")
            };

            // save to db
            await dbContext.Roles.AddRangeAsync(roles);

        }

        private User createSingleStudent(string Username, string Firstname, string Lastname)
        {
            var (salt, hashedPassword) = passwordHelper.GenerateSaltAndHash("defaultPass");
            User user = new User()
            {                
                Username = Username,
                Firstname = Firstname,
                Lastname = Lastname,
                Password = hashedPassword,
                PasswordSalt = salt                
            };
            return user;
        }

        private Role createSingleRole( string Role)
        {
            Role role = new Role()
            {
                
                Name = Role,
            };
            return role;
        }
    }
}
