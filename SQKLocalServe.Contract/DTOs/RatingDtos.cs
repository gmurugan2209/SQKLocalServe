namespace SQKLocalServe.Contract.DTOs;

public class CreateRatingDto
{
    public int BookingId { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
}

public class RatingDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int ServiceId { get; set; }
    public string ServiceName { get; set; }
    public int ExecutiveId { get; set; }
    public string ExecutiveName { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RatingSummaryDto
{
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public List<RatingDto> Ratings { get; set; }

    public RatingSummaryDto()
    {
        Ratings = new List<RatingDto>();
    }

}