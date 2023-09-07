namespace StudentManagement.Models.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string? Photo { get; set; }
    }
}
