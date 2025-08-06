using FluentValidation;
using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe.Contract.Validators;

public class LoginDtoValidator : AbstractValidator<LoginRequestDTO>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}