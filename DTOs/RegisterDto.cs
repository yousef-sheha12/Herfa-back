using Herfa_back.Models;
using System.ComponentModel.DataAnnotations;

namespace Herfa_back.DTOs
{
    public class RegisterDto
    {
        public string UserName {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole role { get; set; }
    }
}
