using Microsoft.AspNetCore.Mvc;
using API.DTOs.UserDTOs;
using static API.DTOs.ServiceResponses;

namespace API.Controllers
{
    public class CustomController : ControllerBase
    {
        protected IActionResult ResponseHTTP(GeneralResponse? response = null)
        {
            switch (response?.statusCode)
            {
                case 200: return Ok(response);
                case 201: return Created("", response);
                case 400: return BadRequest(response);
                case 401: return Unauthorized(response);
                case 403: return Forbid();
                case 404: return NotFound(response);
                case 409: return Conflict(response);
                default: return response is null ? StatusCode(500, "Internal Server Error") : StatusCode(500, response);
            }
        }
    }
}
