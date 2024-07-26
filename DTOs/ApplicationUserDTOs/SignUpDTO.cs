using System.ComponentModel.DataAnnotations;
namespace API.DTOs.UserDTOs
{
    public class SignUpDTO
    {

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public required string Role {  get; set; } = string.Empty;
    }
}
