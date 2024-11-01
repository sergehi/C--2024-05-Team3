namespace AuthorizationService.Core.Entities
{
    public class TokenSettings
    {
        public Guid Id { get; set; }
        public int AccessTokenExpiryMinutes { get; set; }
        public int RefreshTokenExpiryDays { get; set; }
    }
}