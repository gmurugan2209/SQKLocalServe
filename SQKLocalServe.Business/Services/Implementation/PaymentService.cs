using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.Common;
using SQKLocalServe.Contract.DTOs;
using SQKLocalServe.DataAccess;
using SQKLocalServe.DataAccess.Entities;

namespace SQKLocalServe.Business.Services.Implementation;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(ApplicationDbContext context, ILogger<PaymentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<PaymentDto>> InitiatePaymentAsync(int userId, InitiatePaymentDto dto)
    {
        try
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId);

            if (booking == null)
                return ApiResponse<PaymentDto>.NotFound("Booking not found");

           
            var payment = new Payment
            {
                BookingId = dto.BookingId,
                UserId = userId,
                TransactionId = Guid.NewGuid().ToString("N"),
                Amount = booking.Amount,
                Status = "Initiated",
                PaymentMethod = dto.PaymentMethod,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return ApiResponse<PaymentDto>.Success(await MapToDto(payment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating payment for booking {BookingId}", dto.BookingId);
            throw;
        }
    }

    public async Task<ApiResponse<PaymentDto>> VerifyPaymentAsync(VerifyPaymentDto dto)
    {
        try
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == dto.TransactionId);

            if (payment == null)
                return ApiResponse<PaymentDto>.NotFound("Payment not found");

            // Simulate payment verification
            payment.Status = dto.GatewayResponse == "success" ? "Completed" : "Failed";
            payment.PaymentGatewayResponse = dto.GatewayResponse;
            payment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Update booking status if payment is successful
            if (payment.Status == "Completed")
            {
                var booking = await _context.Bookings.FindAsync(payment.BookingId);
                if (booking != null)
                {
                    booking.Status = "Confirmed";
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            return ApiResponse<PaymentDto>.Success(await MapToDto(payment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment {TransactionId}", dto.TransactionId);
            throw;
        }
    }

    public async Task<ApiResponse<List<PaymentDto>>> GetUserPaymentsAsync(int userId)
    {
        try
        {
            var payments = await _context.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var paymentDtos = await Task.WhenAll(payments.Select(MapToDto));
            return ApiResponse<List<PaymentDto>>.Success(paymentDtos.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for user {UserId}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<PaymentDto>> GetBookingPaymentAsync(int bookingId)
    {
        try
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment == null)
                return ApiResponse<PaymentDto>.NotFound("Payment not found for this booking");

            return ApiResponse<PaymentDto>.Success(await MapToDto(payment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment for booking {BookingId}", bookingId);
            throw;
        }
    }

    private async Task<PaymentDto> MapToDto(Payment payment)
    {
        await _context.Entry(payment)
            .Reference(p => p.Booking)
            .LoadAsync();

        return new PaymentDto
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            UserId = payment.UserId,
            TransactionId = payment.TransactionId,
            Amount = payment.Amount,
            Status = payment.Status,
            PaymentMethod = payment.PaymentMethod,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            Booking = await MapBookingToDto(payment.Booking)
        };
    }

    private static Task<BookingDto> MapBookingToDto(Booking booking)
    {
        // Implement booking mapping logic here
        if (booking == null)
            return Task.FromResult<BookingDto>(null);

        var bookingDto = new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            Amount = booking.Amount,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
            // Add other properties as needed
        };

        return Task.FromResult(bookingDto);
    }

}