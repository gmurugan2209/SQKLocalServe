namespace SQKLocalServe.Contract.DTOs;

public class InitiatePaymentDto
{
    public int BookingId { get; set; }
    public string PaymentMethod { get; set; }
}

public class VerifyPaymentDto
{
    public string TransactionId { get; set; }
    public string GatewayResponse { get; set; }
}

public class PaymentDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public BookingDto Booking { get; set; }
}