namespace UserManagementService.Application.DTOs
{
    public class UpdateUserDto
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public string? Role { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}
