using System.ComponentModel.DataAnnotations;
namespace API.DTOs.UserDTOs
{
    public class LoginResponseDTO : UserInfo
    {
        public string? Token { get; set; } = string.Empty;

    }
}
