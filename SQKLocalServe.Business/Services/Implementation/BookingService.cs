using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.Common;
using SQKLocalServe.Contract.DTOs;
using SQKLocalServe.DataAccess;
using SQKLocalServe.DataAccess.Entities;

namespace SQKLocalServe.Business.Services.Implementation;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BookingService> _logger;

    public BookingService(ApplicationDbContext context, ILogger<BookingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<BookingDto>> CreateBookingAsync(int userId, CreateBookingDto dto)
    {
        try
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == dto.ServiceId && s.IsActive);
                
            if (service == null)
                return ApiResponse<BookingDto>.NotFound("Service not found or inactive");

            var booking = new Booking
            {
                UserId = userId,
                ServiceId = dto.ServiceId,
                BookingDate = dto.BookingDate,
                Status = "Pending",
                Amount = service.Price,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Creating new booking for user {UserId} and service {ServiceId}", userId, dto.ServiceId);
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            return ApiResponse<BookingDto>.Success(await MapToDto(booking));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user {UserId}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<BookingDto>> GetBookingByIdAsync(int id)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return ApiResponse<BookingDto>.NotFound("Booking not found");

            return ApiResponse<BookingDto>.Success(await MapToDto(booking));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking {BookingId}", id);
            throw;
        }
    }

    public async Task<ApiResponse<BookingDto>> UpdateBookingStatusAsync(int id, UpdateBookingStatusDto dto)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return ApiResponse<BookingDto>.NotFound("Booking not found");

            booking.Status = dto.Status;
            booking.Notes = dto.Notes ?? booking.Notes;
            booking.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Updating booking {BookingId} status to {Status}", id, dto.Status);
            await _context.SaveChangesAsync();

            return ApiResponse<BookingDto>.Success(await MapToDto(booking));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking {BookingId} status", id);
            throw;
        }
    }

    public async Task<ApiResponse<List<BookingDto>>> GetUserBookingsAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Service)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var bookingDtos = await Task.WhenAll(items.Select(MapToDto));

            var pagedList = bookingDtos.ToList();
            
            return ApiResponse<List<BookingDto>>.Success(pagedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user {UserId}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<List<BookingDto>>> GetAllBookingsAsync(BookingFilterDto filter)
    {
        try
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Service)
                .AsQueryable();

            // Apply filters
            if (filter.FromDate.HasValue)
                query = query.Where(b => b.BookingDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(b => b.BookingDate <= filter.ToDate.Value);

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(b => b.Status == filter.Status);

            query = query.OrderByDescending(b => b.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var bookingDtos = await Task.WhenAll(items.Select(MapToDto));

            var pagedList = bookingDtos.ToList();
            
            return ApiResponse<List<BookingDto>>.Success(pagedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings with filter {@Filter}", filter);
            throw;
        }
    }

    private async Task<BookingDto> MapToDto(Booking booking)
    {
        await _context.Entry(booking)
            .Reference(b => b.User)
            .LoadAsync();
        await _context.Entry(booking)
            .Reference(b => b.Service)
            .LoadAsync();

        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            UserName = $"{booking.User.FullName}",
            ServiceId = booking.ServiceId,
            ServiceName = booking.Service.Name,
            BookingDate = booking.BookingDate,
            Status = booking.Status,
            Amount = booking.Amount,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }
}