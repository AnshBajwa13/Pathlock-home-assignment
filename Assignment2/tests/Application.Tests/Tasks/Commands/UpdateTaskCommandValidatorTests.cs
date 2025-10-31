using Application.Tasks.Commands.UpdateTask;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Application.Tests.Tasks.Commands;

/// <summary>
/// Tests for UpdateTaskCommandValidator to ensure proper validation rules
/// </summary>
public class UpdateTaskCommandValidatorTests
{
    private readonly UpdateTaskCommandValidator _validator;

    public UpdateTaskCommandValidatorTests()
    {
        _validator = new UpdateTaskCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_All_Fields_Are_Valid()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Title = "Updated Task Title",
            Description = "Updated description",
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Id_Is_Empty()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.Empty, // Invalid
            Title = "Valid Title"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Task ID is required"); // Fixed to match actual
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Title = "" // Invalid
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_200_Characters()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Title = new string('X', 201) // Invalid: too long
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Pass_When_Description_Is_Null()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Description = null // Valid: optional field
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Fail_When_DueDate_Is_In_Past()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            DueDate = DateTime.UtcNow.Date.AddDays(-5) // Invalid: past date
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }
}
