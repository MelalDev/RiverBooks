using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.EmailSending.Integrations;
using Serilog;

namespace RiverBooks.EmailSending;

public static class EmailSendingModuleServicesExtensions
{
    public static IServiceCollection AddEmailSendingModuleServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        List<Assembly> mediatRAssemsblies)
    {
        // Add module services
        services.AddTransient<ISendEmail, MimeKitEmailSender>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemsblies.Add(typeof(EmailSendingModuleServicesExtensions).Assembly);

        logger.Information("{Module} module services registered", "Email Sending");

        return services;
    }
}