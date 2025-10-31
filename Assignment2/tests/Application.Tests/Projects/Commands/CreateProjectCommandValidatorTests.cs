using Application.Projects.Commands.CreateProject;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Application.Tests.Projects.Commands;

/// <summary>
/// Tests for CreateProjectCommandValidator
/// Why this matters: Ensures project titles are meaningful and descriptions are reasonable length
/// </summary>
public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator;

    public CreateProjectCommandValidatorTests()
    {
        _validator = new CreateProjectCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Title_And_Description_Are_Valid()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = "Website Redesign Project",
            Description = "Complete overhaul of company website with modern UI/UX"
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
        var command = new CreateProjectCommand
        {
            Title = "", // Invalid
            Description = "Some description"
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
        var command = new CreateProjectCommand
        {
            Title = new string('A', 201), // Invalid: too long
            Description = "Description"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must not exceed 200 characters");
    }

    [Fact]
    public void Should_Pass_When_Description_Is_Null()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = "Valid Project",
            Description = null! // Valid: description is optional
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
        var command = new CreateProjectCommand
        {
            Title = "Valid Project",
            Description = new string('B', 1001) // Invalid: too long
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 1000 characters");
    }

    [Fact]
    public void Should_Pass_With_Maximum_Valid_Lengths()
    {
        // Arrange - Testing boundary conditions
        var command = new CreateProjectCommand
        {
            Title = new string('A', 200), // Exactly 200 characters (max)
            Description = new string('B', 1000) // Exactly 1000 characters (max)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
