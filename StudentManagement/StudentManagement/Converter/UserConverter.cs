using StudentManagement.Models.Dtos;
using StudentManagement.Models.Entities;

namespace StudentManagement.Converter
{
    public class UserConverter
    {
        public List<UserDto> convertEntitiesToDtos(List<User> userEntities)
        {
            List<UserDto> convertedResult = userEntities.Select(
                    user => ConvertEntityToDto(user)
                ).ToList();
            return convertedResult;
        }

        public UserDto ConvertEntityToDto(User user)
        {
            var dto = new UserDto();
            dto.Id = user.Id;
            dto.Username = user.Username;
            dto.Firstname = user.Firstname;
            dto.Lastname = user.Lastname;
            if (user.Photo != null)
            {
                dto.Photo = Convert.ToBase64String(user.Photo);
            }
            return dto;
        }

    }
}
