using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SQKLocalServe.Contract.DTOs;
using SQKLocalServe.DataAccess;

namespace SQKLocalServe.Contract.Validators;

public class InitiatePaymentDtoValidator : AbstractValidator<InitiatePaymentDto>
{
    public InitiatePaymentDtoValidator(ApplicationDbContext context)
    {
        RuleFor(x => x.BookingId)
            .MustAsync(async (bookingId, _) =>
                await context.Bookings.AnyAsync(b => b.Id == bookingId && b.Status == "Confirmed"))
            .WithMessage("Payment can only be initiated for confirmed bookings");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .Must(x => new[] { "CreditCard", "DebitCard", "NetBanking", "UPI" }.Contains(x))
            .WithMessage("Invalid payment method");
    }
}

public class VerifyPaymentDtoValidator : AbstractValidator<VerifyPaymentDto>
{
    public VerifyPaymentDtoValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .WithMessage("Transaction ID is required");

        RuleFor(x => x.GatewayResponse)
            .NotEmpty()
            .WithMessage("Gateway response is required");
    }
}