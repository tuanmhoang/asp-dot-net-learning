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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<UserDto>> GetStudents(int page, int limit)
        {
            if (page == 0)
            {
                page = 1;
            }
                
            if (limit == 0)
            {
                limit = int.MaxValue;
            }
               

            var skip = (page - 1) * limit;

            var userEntities = await dbContext.Users.Skip(skip).Take(limit).ToListAsync();
            var userDtos = userConverter.convertEntitiesToDtos(userEntities);
            return Ok(userDtos);
        }

        /// <summary>
        /// Find student by ID
        /// </summary>
        /// <remarks>This API helps to find a specific student by ID.</remarks>
        /// <response code="200">Everything is OK</response>
        /// <response code="500">Oops! Something is wrong right now</response>
        [HttpGet("{id}")]
 
        public async Task<ActionResult<UserDto>> FindStudentById(int id)
        {
            var foundStudent = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(foundStudent == null)
            {
                return NotFound("Student not found.");
            }
            return Ok(userConverter.ConvertEntityToDto(foundStudent));
        }

        /// <summary>
        /// Update student by ID
        /// </summary>
        /// <remarks>This API helps to update a student info by ID.</remarks>
        /// <response code="200">Everything is OK</response>
        /// <response code="500">Oops! Something is wrong right now</response>
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

            UserDto updatedStudentDto = userConverter.ConvertEntityToDto(studentToUpdate);

            return Ok(updatedStudentDto);
        }

        /// <summary>
        /// Update a student photo
        /// </summary>
        /// <remarks>This API helps to update a student photo.</remarks>
        /// <response code="200">Everything is OK</response>
        /// <response code="500">Oops! Something is wrong right now</response>
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

            UserDto updatedStudentDto = userConverter.ConvertEntityToDto(updatedStudent);

            return Ok(updatedStudentDto);
        }

    }
}
