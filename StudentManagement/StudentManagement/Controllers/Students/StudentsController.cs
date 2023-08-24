using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [SwaggerOperation(Summary = "Update a student info")]
        [HttpPatch("{id}")]
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

            UserDto updatedStudentDto = ConvertEntityToDto(studentToUpdate);

            return Ok(updatedStudentDto);
        }

        [SwaggerOperation(Summary = "Update a student photo")]
        [HttpPut("{id}/photo")]
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

            UserDto updatedStudentDto = ConvertEntityToDto(updatedStudent);

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
