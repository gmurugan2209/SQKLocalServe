namespace SQKLocalServe.Common;

public class ApiResponse<T>
{
    // Removed unused field and constructor

    public int ResponseCode { get; set; }
    public string ResponseDetailCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public T? Data { get; set; }

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

    public static ApiResponse<T> Failed(string detailCode, string message, int statusCode = 400)
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
        return new ApiResponse<T>
        {
            ResponseCode = 404,
            ResponseDetailCode = "NOT_FOUND",
            Description = message,
            Data = default
        };
    }

    public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
    {
        return new ApiResponse<T>
        {
            ResponseCode = 401,
            ResponseDetailCode = "UNAUTHORIZED",
            Description = message,
            Data = default
        };
    }

    public static ApiResponse<T> ValidationError(string message = "Validation failed")
    {
        return new ApiResponse<T>
        {
            ResponseCode = 422,
            ResponseDetailCode = "VALIDATION_ERROR",
            Description = message,
            Data = default
        };
    }

    public static ApiResponse<T> ServerError(string message = "Internal server error")
    {
        return new ApiResponse<T>
        {
            ResponseCode = 500,
            ResponseDetailCode = "SERVER_ERROR",
            Description = message,
            Data = default
        };
    }
    public static ApiResponse<T> Error(string message)
    {
        return new ApiResponse<T>
        {
            ResponseCode = 500,
            ResponseDetailCode = "ERROR",
            Description = message,
            Data = default
        };
    }
}
