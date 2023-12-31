﻿using Microsoft.AspNetCore.Mvc;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugsController : ControllerBase
    {
        [HttpGet(Name = "ConStr")]
        public string Get()
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

            var connectionString = string.Empty;
            if (dbHost == null && dbName == null && dbPassword == null)
            {
                connectionString = "Server=localhost;Database=StudentDb;User Id=sa;Password=Hello@135;MultipleActiveResultSets=true;Trusted_Connection=False;TrustServerCertificate=True";
            }
            else
            {
                connectionString = $"Server={dbHost};Database={dbName};User ID=sa;Password={dbPassword};MultipleActiveResultSets=true;Trusted_Connection=False;TrustServerCertificate=True";
            }
            return connectionString;
        }
    }
}
