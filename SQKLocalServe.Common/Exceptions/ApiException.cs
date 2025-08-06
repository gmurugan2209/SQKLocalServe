namespace SQKLocalServe.Common.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public string DetailCode { get; }

    public ApiException(string message, int statusCode = 400, string detailCode = "BAD_REQUEST") 
        : base(message)
    {
        StatusCode = statusCode;
        DetailCode = detailCode;
    }

    public static ApiException NotFound(string message = "Resource not found")
    {
        return new ApiException(message, 404, "NOT_FOUND");
    }

    public static ApiException Unauthorized(string message = "Unauthorized access")
    {
        return new ApiException(message, 401, "UNAUTHORIZED");
    }

    public static ApiException ValidationError(string message = "Validation failed")
    {
        return new ApiException(message, 400, "VALIDATION_ERROR");
    }
}
