using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models.Dtos;
using StudentManagement.Models.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace StudentManagement.Controllers.Students
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentManagementContext dbContext;

        public StudentsController(StudentManagementContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [SwaggerOperation(Summary = "Get all students")]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetStudents()
        {
            var userEntities = await dbContext.Users.ToListAsync();
            var userDtos = convertEntitiesToDtos(userEntities);
            return Ok(userDtos);
        }

        [SwaggerOperation(Summary = "Find a specific student by ID")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> FindStudentById(int id)
        {
            var foundStudent = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(foundStudent == null)
            {
                return NotFound("Student not found.");
            }
            return Ok(ConvertEntityToDto(foundStudent));
        }

        [SwaggerOperation(Summary = "Update a student")]
        [HttpPost]
        public async Task<IActionResult> UpdateStudent(UserDto userDto)
        {
            if(userDto == null)
            {
                return BadRequest("Invalid input.");
            }
            var foundStudent = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id);
            if(foundStudent == null)
            {
                return NotFound("Student not found.");
            }

            foundStudent.Firstname = userDto.Firstname;
            foundStudent.Lastname = userDto.Lastname;
            foundStudent.Photo = userDto.Photo;

            await dbContext.SaveChangesAsync();

            UserDto updatedStudentDto = ConvertEntityToDto(foundStudent);

            return Ok(updatedStudentDto);
        }

        private List<UserDto> convertEntitiesToDtos(List<User> userEntities)
        {
            List<UserDto> convertedResult = userEntities.Select(
                    user => ConvertEntityToDto(user)
                ).ToList();
            return convertedResult;
        }

        private UserDto ConvertEntityToDto(User user)
        {
            var dto = new UserDto();
            dto.Id = user.Id;
            dto.Username = user.Username;
            dto.Firstname = user.Firstname;
            dto.Lastname = user.Lastname;
            dto.Photo = user.Photo;
            return dto;
        }

    }
}
