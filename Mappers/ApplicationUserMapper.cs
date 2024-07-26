using API.Models;
using Riok.Mapperly.Abstractions;
using API.DTOs.UserDTOs;

namespace API.Mappers
{
    [Mapper(UseReferenceHandling = true)]
    public static partial class ApplicationUserMapper
    {
        public static partial ICollection<UserInfo> ApplicationUser_To_ReadDTOList(ICollection<ApplicationUser> createDTO);
        public static partial UserInfo ApplicationUser_To_ReadDTO(ApplicationUser readDTO);
        public static partial LoginResponseDTO ApplicationUser_To_LoginDTO(ApplicationUser readDTO);
        public static partial ApplicationUser ReadDTO_To_ApplicationUser(UserInfo readDTO);




    }
}
