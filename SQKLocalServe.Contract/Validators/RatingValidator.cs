using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SQKLocalServe.Contract.DTOs;
using SQKLocalServe.DataAccess;

namespace sqklocalserve.Contract.Validators;

public class CreateRatingDtoValidator : AbstractValidator<CreateRatingDto>
{
    public CreateRatingDtoValidator(ApplicationDbContext context)
    {
        RuleFor(x => x.BookingId)
            .MustAsync(async (bookingId, _) =>
                await context.Bookings.AnyAsync(b => b.Id == bookingId && b.Status == "Completed"))
            .WithMessage("Rating can only be submitted for completed bookings");

        RuleFor(x => x.Stars)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5 stars");

        RuleFor(x => x.Comment)
            .MaximumLength(500)
            .When(x => x.Comment != null)
            .WithMessage("Comment cannot exceed 500 characters");
    }
}