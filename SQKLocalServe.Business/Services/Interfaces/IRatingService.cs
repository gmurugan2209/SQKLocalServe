using SQKLocalServe.Common;
using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe.Business.Services.Interfaces;

public interface IRatingService
{
    Task<ApiResponse<RatingDto>> CreateRatingAsync(int userId, CreateRatingDto dto);
    Task<ApiResponse<RatingSummaryDto>> GetServiceRatingsAsync(int serviceId, int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<RatingSummaryDto>> GetExecutiveRatingsAsync(int executiveId, int pageNumber = 1, int pageSize = 10);
}