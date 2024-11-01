namespace AuthorizationService.Shared.DTOs
{
    public class LoginDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}