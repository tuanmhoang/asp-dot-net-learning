using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Converter;
using StudentManagement.Models.Dtos;
using StudentManagement.Models.Entities;
using StudentManagement.Models.Requests;
using Swashbuckle.AspNetCore.Annotations;


namespace StudentManagement.Controllers.Students
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentManagementContext dbContext;
        private UserConverter userConverter;


        public StudentsController(StudentManagementContext dbContext)
        {
            this.dbContext = dbContext;
            userConverter = new UserConverter();
        }

        [SwaggerOperation(Summary = "Get all students")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetStudents()
        {
            var userEntities = await dbContext.Users.ToListAsync();
            var userDtos = userConverter.convertEntitiesToDtos(userEntities);
            return Ok(userDtos);
        }

        [SwaggerOperation(Summary = "Find a specific student by ID")]
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> FindStudentById(int id)
        {
            var foundStudent = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(foundStudent == null)
            {
                return NotFound("Student not found.");
            }
            return Ok(userConverter.ConvertEntityToDto(foundStudent));
        }

        [SwaggerOperation(Summary = "Update a student info")]
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] JsonPatchDocument<User> updateRequest)
        {
            if(updateRequest == null)
            {
                return BadRequest("Request is invalid.");
            }

            var studentToUpdate = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(studentToUpdate == null)
            {
                return NotFound("Student not found.");
            }

            updateRequest.ApplyTo(studentToUpdate, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dbContext.Users.Update(studentToUpdate);
            await dbContext.SaveChangesAsync();

            UserDto updatedStudentDto = userConverter.ConvertEntityToDto(studentToUpdate);

            return Ok(updatedStudentDto);
        }

        [SwaggerOperation(Summary = "Update a student photo")]
        [HttpPut("{id}/photo")]
        [Authorize]
        public async Task<IActionResult> UpdateStudentPhoto(int id, [FromForm] UpdateStudentPhotoRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid input.");
            }

            var updatedStudent = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (updatedStudent == null)
            {
                return NotFound("Student not found.");
            }

            using (var memoryStream = new MemoryStream())
            {
                await request.Photo.CopyToAsync(memoryStream);
                var photoData = memoryStream.ToArray();
                updatedStudent.Photo = photoData;
            }

            dbContext.Users.Update(updatedStudent);
            await dbContext.SaveChangesAsync();

            UserDto updatedStudentDto = userConverter.ConvertEntityToDto(updatedStudent);

            return Ok(updatedStudentDto);
        }

    }
}
