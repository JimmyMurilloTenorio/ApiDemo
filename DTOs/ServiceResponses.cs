using API.DTOs.UserDTOs;

namespace API.DTOs;

public class ServiceResponses
{
    public record class GeneralResponse(int? statusCode = null, string? Message = null, object? result = null);
}
