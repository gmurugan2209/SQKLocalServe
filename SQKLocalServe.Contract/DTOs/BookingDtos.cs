namespace SQKLocalServe.Contract.DTOs;

public class CreateBookingDto
{
    public int ServiceId { get; set; }
    public DateTime BookingDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateBookingStatusDto
{
    public string Status { get; set; }
    public string? Notes { get; set; }
}

public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int ServiceId { get; set; }
    public string ServiceName { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BookingFilterDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}