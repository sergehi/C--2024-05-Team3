using Microsoft.AspNetCore.Identity;
using Common.Attributes;

namespace AuthorizationService.Core.Entities
{
    [Guid("123e4567-e89b-12d3-a456-426614174000")]
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
