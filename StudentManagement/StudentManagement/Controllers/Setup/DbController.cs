using Microsoft.AspNetCore.Mvc;
using StudentManagement.Helpers;
using StudentManagement.Models.Entities;

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

        /// <summary>
        /// Init fresh DB
        /// </summary>
        /// <remarks>This API drops all data in DB and recreates fresh DB. 
        /// 
        /// ***** Use this when you want a new DB with no data. *****</remarks>
        /// <response code="200">Everything is OK</response>
        /// <response code="500">Oops! Something is wrong right now</response>
        [HttpGet("fresh")]
        public async Task<ActionResult<string>> CreateDatabase()
        {
            /*            bool deleted = false;
                        bool canConnect = await dbContext.Database.CanConnectAsync();
                        if (canConnect)
                        {
                            deleted= await dbContext.Database.EnsureDeletedAsync();
                        }

                        bool created = await dbContext.Database.EnsureCreatedAsync();*/


            bool created = await dbContext.Database.EnsureCreatedAsync();
            bool deleted = false;
            if (!created)
            {
                deleted = await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.EnsureCreatedAsync();
            }

            string result = $"Is DB deleted: {deleted} - Is DB created: {created}";
            return Ok(result);
        }

        /// <summary>
        /// Init mock data
        /// </summary>
        /// <remarks>This API helps to delete all Data in DB and provide some initial mock data. 
        /// 
        /// ***** Use this when you want a new DB with some mock data. *****</remarks>
        /// <response code="200">Everything is OK</response>
        /// <response code="500">Oops! Something is wrong right now</response>
        [HttpGet("mock")]
        public async Task<ActionResult<string>> CreateMockData()
        {
            await CreateDatabase();
            await CreateMockDataForTesting();

            await dbContext.SaveChangesAsync();
            return Ok("Done creating mock data");
        }

        private async Task CreateMockDataForTesting()
        {
            await CreateMockRoles();
            await CreateMockStudents();
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
            //string txtPath = Path.Combine(Environment.CurrentDirectory, "Resources/avatar.png");
            //byte[] data = System.IO.File.ReadAllBytes(@"Resources/avatar.png");
            //byte[] data = System.IO.File.ReadAllBytes(txtPath);
            User user = new User()
            {                
                Username = Username,
                Firstname = Firstname,
                Lastname = Lastname,
                Password = hashedPassword,
                PasswordSalt = salt,
                //Photo = data
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
