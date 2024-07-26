using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Contracts;
using API.DTOs.UserDTOs;
using static API.DTOs.ServiceResponses;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserAccount userAccount) : CustomController
    {
        /// <summary>
        /// Registers a new user. Only exists "Admin" and "User" roles
        /// </summary>
        /// <param name="userDTO">The user details for registration.</param>
        /// <returns>A response indicating the result of the registration.</returns>
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpDTO userDTO)
        {
            var response = await userAccount.SignUp(userDTO);
            return ResponseHTTP(response);
        }

        /// <summary>
        /// Login using email and password
        /// </summary>
        /// <param name="loginDTO">The user details for login.</param>
        /// <returns>A response indicating the result.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LoginDTO loginDTO)
        {
            var response = await userAccount.LogIn(loginDTO);
            return ResponseHTTP(response);
        }


        /// <summary>
        /// Retrieve a user by ID. (You must be logged in and include the token in the header as 'bearer xxxxxxxx...'). 
        /// Users with the 'User' role can only retrieve their own information, while users with the 'Admin' role can retrieve information for any ID.
        /// </summary>
        /// <param name="id">The user Id</param>
        /// <returns>A response indicating the result.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string id)
        {
            var response = await userAccount.GetUserById(id);
            return ResponseHTTP(response);
        }

        /// <summary>
        /// Retrieve all users in db, (Admin only)
        /// </summary>
        /// <returns>A response indicating the result.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var response = await userAccount.GetUsers();
            return ResponseHTTP(response);
        }
    }
}
