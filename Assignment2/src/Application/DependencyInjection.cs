using System.Reflection;
using Application.Common.Behaviors;
using Application.Scheduling.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Dependency injection configuration for Application layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR - registers all handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // FluentValidation - registers all validators from this assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // MediatR Pipeline Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Scheduling Services
        services.AddScoped<ISchedulingAlgorithm, SchedulingAlgorithm>();

        // AutoMapper - will add profiles when needed
        // services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
