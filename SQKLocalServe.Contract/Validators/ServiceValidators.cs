using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SQKLocalServe.DataAccess;

public class CreateServiceDtoValidator : AbstractValidator<CreateServiceDto>
{
    public CreateServiceDtoValidator(SQKLocalServe.DataAccess.ApplicationDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.CategoryId)
            .MustAsync(async (categoryId, _) => 
                await context.Categories.AnyAsync(c => c.Id == categoryId))
            .WithMessage("Category does not exist");
    }
}

public class UpdateServiceDtoValidator : AbstractValidator<UpdateServiceDto>
{
    public UpdateServiceDtoValidator(ApplicationDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.CategoryId)
            .MustAsync(async (categoryId, _) => 
                await context.Categories.AnyAsync(c => c.Id == categoryId))
            .WithMessage("Category does not exist");
    }
}