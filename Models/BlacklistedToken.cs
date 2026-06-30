namespace Herfa_back.Models
{
    public class BlacklistedToken : BaseEntity
    {
        public string Jti { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime BlacklistedAt { get; set; } = DateTime.UtcNow;
    }
}
