using System.Text.Json.Serialization;

namespace sqklocalserve.Contract.Models;

public class ApiResponse<T>
{
    public int ResponseCode { get; set; }
    public string ResponseDetailCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public T? Data { get; set; }

    [JsonIgnore]
    public bool IsSuccess => ResponseCode == 200;

    public static ApiResponse<T> Success(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            ResponseCode = 200,
            ResponseDetailCode = "SUCCESS",
            Description = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(string detailCode, string message, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            ResponseCode = statusCode,
            ResponseDetailCode = detailCode,
            Description = message,
            Data = default
        };
    }

    public static ApiResponse<T> NotFound(string message = "Resource not found")
    {
        return Fail("NOT_FOUND", message, 404);
    }

    public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
    {
        return Fail("UNAUTHORIZED", message, 401);
    }

    public static ApiResponse<T> ValidationError(string message = "Validation failed")
    {
        return Fail("VALIDATION_ERROR", message, 400);
    }

    public static ApiResponse<T> ServerError(string message = "An unexpected error occurred")
    {
        return Fail("SERVER_ERROR", message, 500);
    }
}
