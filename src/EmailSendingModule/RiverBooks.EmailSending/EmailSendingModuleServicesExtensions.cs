using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using MongoDB.Driver;
using RiverBooks.EmailSending.Integrations;
using RiverBooks.EmailSending.EmailBackgroundService;

namespace RiverBooks.EmailSending;

public static class EmailSendingModuleServicesExtensions
{
    public static IServiceCollection AddEmailSendingModuleServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        List<Assembly> mediatRAssemsblies)
    {
        //configure MongoDB
        services.Configure<MongoDBSettings>(config.GetSection("MongoDB"));
        services.AddMongoDB(config);

        // Add module services
        services.AddTransient<ISendEmail, MimeKitEmailSender>();
        //services.AddTransient<IOutboxService, MongoDbOutBoxService>();
        services.AddTransient<IQueueEmailsInOutboxService, MongoDbQueueEmailOutboxService>();
        services.AddTransient<IGetEmailsFromOutboxService, MongoDbGetEmailsFromOutboxOutBoxService>();
        services.AddTransient<ISendEmailsFromOutboxService, DefaultSendEmailsFromOutboxService>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemsblies.Add(typeof(EmailSendingModuleServicesExtensions).Assembly);

        // Add Background worker
        services.AddHostedService<EmailSendingBackgroundService>();

        logger.Information("{Module} module services registered", "Email Sending");

        return services;
    }

    public static IServiceCollection AddMongoDB(this IServiceCollection services,
    IConfiguration configuration)
    {
        // Register the MongoDB client as a singleton
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
            return new MongoClient(settings!.ConnectionString);
        });

        // Register the MongoDB database as a singleton
        services.AddSingleton(serviceProvider =>
        {
            var settings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
            var client = serviceProvider.GetService<IMongoClient>();
            return client!.GetDatabase(settings!.DatabaseName);
        });

        //// Optionally, register specific collections here as scoped or singleton services
        //// Example for a 'EmailOutboxEntity' collection
        services.AddTransient(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return database!.GetCollection<EmailOutboxEntity>("EmailOutboxEntityCollection");
        });

        return services;
    }
}
