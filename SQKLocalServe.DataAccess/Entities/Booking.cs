namespace SQKLocalServe.DataAccess.Entities;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } // Pending, Confirmed, InProgress, Completed, Cancelled
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Service Service { get; set; }

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}