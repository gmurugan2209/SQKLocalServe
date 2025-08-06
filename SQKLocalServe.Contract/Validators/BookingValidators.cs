using FluentValidation;
using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe.Contract.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0)
            .WithMessage("A valid service must be selected");

        RuleFor(x => x.BookingDate)
            .GreaterThan(DateTime.Now)
            .WithMessage("Booking date must be in the future");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => x.Notes != null);
    }
}

public class UpdateBookingStatusDtoValidator : AbstractValidator<UpdateBookingStatusDto>
{
    public UpdateBookingStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(x => new[] { "Pending", "Confirmed", "InProgress", "Completed", "Cancelled" }.Contains(x))
            .WithMessage("Invalid booking status");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => x.Notes != null);
    }
}