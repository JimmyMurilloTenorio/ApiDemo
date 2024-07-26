using API.Contracts;
using API.DTOs.UserDTOs;
using API.Mappers;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static API.DTOs.ServiceResponses;
namespace API.Repositories
{
    public class AccountRepository(
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration config
    ) : IUserAccount
    {
        public async Task<GeneralResponse> SignUp(SignUpDTO signUpDTO)
        {
            try
            {
                var newUser = new ApplicationUser()
                {
                    Name = signUpDTO.Name,
                    Email = signUpDTO.Email,
                    UserName = signUpDTO.Email,
                };

                var user = await userManager.FindByEmailAsync(newUser.Email);
                if (user is not null) return new GeneralResponse(409, "El usuario ya existe");

                var roleExists = await roleManager.RoleExistsAsync(signUpDTO.Role);
                if (!roleExists) return new GeneralResponse(404, $"El role no existe");

                var createUser = await userManager.CreateAsync(newUser!, signUpDTO.Password);
                if (!createUser.Succeeded) return new GeneralResponse(500, "No fue posible crear el usuario", createUser.Errors);


                await userManager.AddToRoleAsync(newUser, signUpDTO.Role);

                var userCreated = ApplicationUserMapper.ApplicationUser_To_ReadDTO(newUser);
                userCreated.Role = signUpDTO.Role;

                return new GeneralResponse(200, "Cuenta creada correctamente", userCreated);
            }
            catch (Exception ex)
            {
                return new GeneralResponse(500, $"No fue posible crear el usuario: {ex.Message}");
            }            
        }

        public async Task<GeneralResponse> LogIn(LoginDTO loginDTO)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(loginDTO.Email);
                if (user is null)
                    return new GeneralResponse(404, "El usuario no existe");

                bool checkUserPasswords = await userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (!checkUserPasswords)
                    return new GeneralResponse(404, "Credenciales incorrectas");

                var userRoles = await userManager.GetRolesAsync(user);

                var readDTO = ApplicationUserMapper.ApplicationUser_To_LoginDTO(user);
                readDTO.Role = userRoles.First();
                string token = GenerateToken(readDTO);
                readDTO.Token = token;

                httpContextAccessor?.HttpContext?.Response.Cookies.Append("X-Access-Token", token, new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return new GeneralResponse(200, "Ha iniciado sesión correctamente", readDTO);
            }
            catch(Exception e)
            {
                return new GeneralResponse(500, $"No fue posible iniciar sesión: {e.Message}");
            }
            
        }

        public async Task<GeneralResponse> GetUserById(string id)
        {
            try
            {
                var requestedById = httpContextAccessor?.HttpContext?.User.FindFirst("id")?.Value;
                if (requestedById == null)
                    return new GeneralResponse(500);

                var requestedByUser = await GetUserInfoById(requestedById);
                if (requestedByUser.Role != "Admin" && requestedById != id)
                    return new GeneralResponse(403, "No tiene permisos para ver la información de otro usuario");

                var user = await GetUserInfoById(id);
                return new GeneralResponse(200, "Usuario recuperado correctamente", user);

            }
            catch (Exception e)
            {
                return new GeneralResponse(500, $"No fue posible recuperear el usuario: {e.Message}");
            }
        }

        private async Task<UserInfo> GetUserInfoById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null)
                throw new Exception("El usuario no existe");
            var userRoles = await userManager.GetRolesAsync(user);
            var readDTO = ApplicationUserMapper.ApplicationUser_To_ReadDTO(user);
            readDTO.Role = userRoles.First();
            return readDTO;
        }

        public async Task<GeneralResponse> GetUsers()
        {
            try
            {
                var users = await userManager.Users
                    .ToListAsync();
                if (users.Count == 0)
                    return new GeneralResponse(404, "No hay usuarios");

                var readDTOList = new List<UserInfo>();
                foreach (var user in users)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    var readDTO = ApplicationUserMapper.ApplicationUser_To_ReadDTO(user);
                    readDTO.Role = userRoles.First();
                    readDTOList.Add(readDTO);
                }


                return new GeneralResponse(200, "Usuarios recuperados correctamente", readDTOList);
            }
            catch (Exception e)
            {
                return new GeneralResponse(500, $"No fue posible recuperear los usuarios: {e.Message}");
            }
        }

        private string GenerateToken(UserInfo user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim("id", user.Id),
                new Claim("name", user.Name),
                new Claim("email", user.Email),
                new Claim("role", user.Role)
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        
    }
}
