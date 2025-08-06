using FluentValidation;
using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe.Contract.Validators;

public class CreateLocationDtoValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and cannot exceed 100 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("City is required and cannot exceed 50 characters");

        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("State is required and cannot exceed 50 characters");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .Matches(@"^\d{6}$")
            .WithMessage("Invalid postal code format (6 digits required)");
    }
}

public class UpdateLocationDtoValidator : AbstractValidator<UpdateLocationDto>
{
    public UpdateLocationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and cannot exceed 100 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("City is required and cannot exceed 50 characters");

        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("State is required and cannot exceed 50 characters");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .Matches(@"^\d{6}$")
            .WithMessage("Invalid postal code format (6 digits required)");
    }
}