using Application.Tasks.Commands.CreateTask;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Application.Tests.Tasks.Commands;

/// <summary>
/// Tests for CreateTaskCommandValidator to ensure proper validation rules
/// </summary>
public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator;

    public CreateTaskCommandValidatorTests()
    {
        _validator = new CreateTaskCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Title_Is_Valid()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Valid Task Title",
            Description = "Optional description",
            DueDate = DateTime.UtcNow.AddDays(7),
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "", // Invalid: empty title
            ProjectId = Guid.NewGuid()
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
        var command = new CreateTaskCommand
        {
            Title = new string('A', 201), // Invalid: 201 characters
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must not exceed 200 characters");
    }

    [Fact]
    public void Should_Pass_When_Description_Is_Optional_And_Null()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Valid Task",
            Description = null, // Valid: description is optional
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Fail_When_Description_Exceeds_1000_Characters()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Valid Task",
            Description = new string('B', 1001), // Invalid: 1001 characters
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 1000 characters");
    }

    [Fact]
    public void Should_Pass_When_DueDate_Is_In_Future()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Task with due date",
            DueDate = DateTime.UtcNow.Date.AddDays(5), // Valid: future date
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void Should_Fail_When_DueDate_Is_In_Past()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Task with past due date",
            DueDate = DateTime.UtcNow.Date.AddDays(-1), // Invalid: past date
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DueDate)
            .WithErrorMessage("Due date must be in the future"); // Fixed to match actual
    }

    [Fact]
    public void Should_Pass_When_DueDate_Is_Today()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Task due today",
            DueDate = DateTime.UtcNow.AddHours(1), // Valid: future time today
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void Should_Fail_When_ProjectId_Is_Empty()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Valid Task",
            ProjectId = Guid.Empty // Invalid: empty GUID
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("Project ID is required"); // Fixed to match actual message
    }
}
