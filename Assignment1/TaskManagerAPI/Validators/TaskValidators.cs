using FluentValidation;
using TaskManagerAPI.Models.DTOs;

namespace TaskManagerAPI.Validators;

/// <summary>
/// Validator for CreateTaskRequest using FluentValidation.
/// FluentValidation is preferred over DataAnnotations because it's more flexible,
/// testable, and supports complex validation scenarios.
/// </summary>
public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MinimumLength(3)
            .WithMessage("Description must be at least 3 characters")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");
    }
}

/// <summary>
/// Validator for UpdateTaskRequest
/// </summary>
public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MinimumLength(3)
            .WithMessage("Description must be at least 3 characters")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");
    }
}
