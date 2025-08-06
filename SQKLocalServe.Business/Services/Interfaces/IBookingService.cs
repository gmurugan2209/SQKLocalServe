using SQKLocalServe.Common;
using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe.Business.Services.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<BookingDto>> CreateBookingAsync(int userId, CreateBookingDto dto);
    Task<ApiResponse<BookingDto>> GetBookingByIdAsync(int id);
    Task<ApiResponse<BookingDto>> UpdateBookingStatusAsync(int id, UpdateBookingStatusDto dto);
    Task<ApiResponse<List<BookingDto>>> GetUserBookingsAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<List<BookingDto>>> GetAllBookingsAsync(BookingFilterDto filter);
}