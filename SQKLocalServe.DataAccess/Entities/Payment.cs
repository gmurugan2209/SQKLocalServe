namespace SQKLocalServe.DataAccess.Entities;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } // Initiated, Pending, Completed, Failed
    public string? PaymentMethod { get; set; }
    public string? PaymentGatewayResponse { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Booking Booking { get; set; }
    public virtual User User { get; set; }
}