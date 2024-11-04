using Common.Attributes;

namespace AuthorizationService.Core.Entities
{
    [Guid("123e4567-e89b-12d3-a456-426614174000")]
    public class User
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}