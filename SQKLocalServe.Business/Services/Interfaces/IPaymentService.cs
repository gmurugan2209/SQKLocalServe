using SQKLocalServe.Common;
using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe.Business.Services.Interfaces;

public interface IPaymentService
{
    Task<ApiResponse<PaymentDto>> InitiatePaymentAsync(int userId, InitiatePaymentDto dto);
    Task<ApiResponse<PaymentDto>> VerifyPaymentAsync(VerifyPaymentDto dto);
    Task<ApiResponse<List<PaymentDto>>> GetUserPaymentsAsync(int userId);
    Task<ApiResponse<PaymentDto>> GetBookingPaymentAsync(int bookingId);
}