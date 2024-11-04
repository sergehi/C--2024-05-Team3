namespace AuthorizationService.Shared.DTOs
{
    public class TokensDTO
    {
        public required string RefreshToken { get; set; }
        public required string AccessToken { get; set; }
    }
}