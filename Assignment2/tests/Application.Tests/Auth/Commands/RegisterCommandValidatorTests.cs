using Application.Auth.Commands.Register;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace Application.Tests.Auth.Commands;

/// <summary>
/// Tests for RegisterCommandValidator to ensure password and email validation
/// Why this matters: Strong password requirements prevent weak credentials
/// </summary>
public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;
    private readonly Mock<IApplicationDbContext> _mockContext;

    public RegisterCommandValidatorTests()
    {
        // Create mock database context
        _mockContext = new Mock<IApplicationDbContext>();
        
        // Setup empty users DbSet with async query support
        var usersData = new List<User>().AsQueryable();
        var mockUsersSet = CreateMockDbSet(usersData);
        
        _mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
        
        _validator = new RegisterCommandValidator(_mockContext.Object);
    }

    private static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(data.Provider));
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
        return mockSet;
    }

    [Fact]
    public async Task Should_Pass_When_All_Fields_Are_Valid()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "SecurePass123!" // Meets all requirements
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Fail_When_FullName_Is_Empty()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FullName = "", // Invalid
            Email = "john@example.com",
            Password = "SecurePass123!"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name is required");
    }

    [Fact]
    public async Task Should_Fail_When_Email_Is_Invalid()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "not-an-email", // Invalid format
            Password = "SecurePass123!"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must be a valid email address");
    }

    [Fact]
    public async Task Should_Fail_When_Password_Is_Too_Short()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "Short1!" // Invalid: less than 8 characters
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Should_Fail_When_Password_Missing_Uppercase()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "lowercase123!" // Invalid: no uppercase
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter");
    }

    [Fact]
    public async Task Should_Fail_When_Password_Missing_Lowercase()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "UPPERCASE123!" // Invalid: no lowercase
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Should_Fail_When_Password_Missing_Number()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "NoNumbersHere!" // Invalid: no digits
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Should_Fail_When_Password_Missing_Special_Character()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "NoSpecial123" // Invalid: no special character
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("ValidPass123!")]
    [InlineData("Str0ng@Password")]
    [InlineData("C0mpl3x#Pass")]
    public async Task Should_Pass_With_Various_Valid_Passwords(string password)
    {
        // Arrange
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = password
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}

// Test helpers for async query support
internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var resultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })
            ?.MakeGenericMethod(resultType)
            .Invoke(this, new object[] { expression });

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
            ?.MakeGenericMethod(resultType)
            .Invoke(null, new[] { executionResult });
    }
}

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return new ValueTask();
    }
}
