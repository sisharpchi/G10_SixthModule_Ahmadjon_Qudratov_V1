using ContactSystem.Bll.Dtos;
using FluentValidation;

namespace ContactSystem.Bll.Validators;

public class ContactDtoValidator : AbstractValidator<ContactDto>
{
    public ContactDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be a positive number.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{9,15}$").WithMessage("Phone number format is invalid.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email address format is invalid.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");
    }
}