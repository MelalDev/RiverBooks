using FluentValidation;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace RiverBooks.SharedKernel;

public static class BehaviourExtensions
{
    public static IServiceCollection AddMediatRLoggingBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }

    public static IServiceCollection AddMediatRFluentValidationBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));

        return services;
    }

    /*
     * Right now, we have a validator inside of the use case folder of ther users module. So we want to make sure that
     * that validator is found when the application starts up so that it will be applied when this behavior runs.
     * We can just do that as another extension method (AddValidatorsFromAssemblyContaining). So this extension method
     * will let you specify a type that you're going to use to determine the assembly and the eveyrhting in that 
     * assembly will be scanned to see if there are any validators. So if there are, then it's going to add them into
     * your service colleciton
     */
    public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(
    this IServiceCollection services)
    {
        // Get the assembly containing the specified type
        var assembly = typeof(T).GetTypeInfo().Assembly;

        // Find all validator types in the assembly
        var validatorTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToList();

        // Register each validator with its implemented interfaces
        foreach (var validatorType in validatorTypes)
        {
            var implementedInterfaces = validatorType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(IValidator<>));

            foreach (var implementedInterface in implementedInterfaces)
            {
                services.AddTransient(implementedInterface, validatorType);
            }
        }

        return services;
    }
}
