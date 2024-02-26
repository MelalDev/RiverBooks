using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Users.Data;
using Serilog;

namespace RiverBooks.Users;

public static class UsersModuleServiceExtensions
{
    public static IServiceCollection AddUserModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> mediatRAssemblies)
    {
        /*
        * we're adding a connection string that's identical to the books connection string because we're talking to the same 
        * database on the same server. The only thing that's going to be different is we're configuring it with a schema just like
        * we did for books.
        */
        string? connectionString = config.GetConnectionString("UsersConnectionString");
        services.AddDbContext<UsersDbContext>(config => config.UseSqlServer(connectionString));

        services.AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<UsersDbContext>();

        //Add MediatR Domain Event Dispatcher
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        // Add User Serives
        services.AddScoped<IApplicationUserRepository, EfApplicationUserRepository>();
        services.AddScoped<IReadOnlyUserStreetAddressRepository, EfUserStreetAddressRepository>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(UsersModuleServiceExtensions).Assembly);

        logger.Information("{Module} module services registered", "Users");

        return services;
    }
}