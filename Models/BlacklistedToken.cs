namespace Herfa_back.Models
{
    public class BlacklistedToken
    {
        public Guid Id { get; set; }
        public string Jti { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime BlacklistedAt { get; set; } = DateTime.UtcNow;
    }
}
