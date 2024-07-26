using API.DTOs.UserDTOs;
using static API.DTOs.ServiceResponses;
namespace API.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> SignUp(SignUpDTO userDTO);
        Task<GeneralResponse> LogIn(LoginDTO loginDTO);
        Task<GeneralResponse> GetUserById(string id);
        Task<GeneralResponse> GetUsers();





    }
}
