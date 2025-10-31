using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

/// <summary>
/// Interface for the application database context
/// Abstracts EF Core DbContext to keep Application layer independent of Infrastructure
/// </summary>
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<TaskDependency> TaskDependencies { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
