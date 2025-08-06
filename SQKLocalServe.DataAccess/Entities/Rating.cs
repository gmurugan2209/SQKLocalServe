namespace SQKLocalServe.DataAccess.Entities;

public class Rating
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public int ExecutiveId { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual Booking Booking { get; set; }
    public virtual User User { get; set; }
    public virtual User Executive { get; set; }
    public virtual Service Service { get; set; }
}