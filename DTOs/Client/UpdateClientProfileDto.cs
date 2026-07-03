using System.ComponentModel.DataAnnotations;

namespace Herfa_back.DTOs.Client
{
    public class UpdateClientProfileDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
    }
}
