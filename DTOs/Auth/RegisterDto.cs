using Herfa_back.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Herfa_back.DTOs.Auth
{
    public class RegisterDto
    {
        public string UserName {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole role { get; set; }
    }
}
