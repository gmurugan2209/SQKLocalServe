using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.Common;
using SQKLocalServe.Contract.DTOs;
using SQKLocalServe.DataAccess;
using SQKLocalServe.DataAccess.Entities;

namespace SQKLocalServe.Business.Services.Implementation;

public class RatingService : IRatingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RatingService> _logger;

    public RatingService(ApplicationDbContext context, ILogger<RatingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<RatingDto>> CreateRatingAsync(int userId, CreateRatingDto dto)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId);

            if (booking == null)
                return ApiResponse<RatingDto>.NotFound("Booking not found");

          
            var rating = new Rating
            {
                BookingId = dto.BookingId,
                UserId = userId,
                ServiceId = booking.ServiceId,
              //  ExecutiveId = booking.Ex,
                Stars = dto.Stars,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();

            return ApiResponse<RatingDto>.Success(await MapToDto(rating));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rating for booking {BookingId}", dto.BookingId);
            throw;
        }
    }

    public async Task<ApiResponse<RatingSummaryDto>> GetServiceRatingsAsync(int serviceId, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var query = _context.Ratings
                .Where(r => r.ServiceId == serviceId)
                .OrderByDescending(r => r.CreatedAt);

            var summary = new RatingSummaryDto
            {
                AverageRating = await query.AverageAsync(r => r.Stars),
                TotalRatings = await query.CountAsync(),
               /* Ratings = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => MapToDto(r))
                    .ToListAsync() */
            };

            return ApiResponse<RatingSummaryDto>.Success(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ratings for service {ServiceId}", serviceId);
            throw;
        }
    }

    public async Task<ApiResponse<RatingSummaryDto>> GetExecutiveRatingsAsync(int executiveId, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var query = _context.Ratings
                .Where(r => r.ExecutiveId == executiveId)
                .OrderByDescending(r => r.CreatedAt);

            var summary = new RatingSummaryDto
            {
                AverageRating = await query.AverageAsync(r => r.Stars),
                TotalRatings = await query.CountAsync(),
               /* Ratings = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => MapToDto(r))
                    .ToListAsync() */
            };

            return ApiResponse<RatingSummaryDto>.Success(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ratings for executive {ExecutiveId}", executiveId);
            throw;
        }
    }

    private async Task<RatingDto> MapToDto(Rating rating)
    {
        await _context.Entry(rating)
            .Reference(r => r.User)
            .LoadAsync();
        await _context.Entry(rating)
            .Reference(r => r.Service)
            .LoadAsync();
        await _context.Entry(rating)
            .Reference(r => r.Executive)
            .LoadAsync();

        return new RatingDto
        {
            Id = rating.Id,
            BookingId = rating.BookingId,
            UserId = rating.UserId,
            UserName = $"{rating.User.FullName}",
            ServiceId = rating.ServiceId,
            ServiceName = rating.Service.Name,
            ExecutiveId = rating.ExecutiveId,
            ExecutiveName = $"{rating.Executive.FullName} {rating.Executive.FullName}",
            Stars = rating.Stars,
            Comment = rating.Comment,
            CreatedAt = rating.CreatedAt
        };
    }
}