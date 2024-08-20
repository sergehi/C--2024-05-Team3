using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}