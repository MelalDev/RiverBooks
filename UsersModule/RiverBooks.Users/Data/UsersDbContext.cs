using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RiverBooks.Users.Data;

public class UsersDbContext : IdentityDbContext
{
    private readonly IDomainEventDispatcher? _dispatcher;
    public UsersDbContext(DbContextOptions<UsersDbContext> options, IDomainEventDispatcher? dispatcher) : base(options)
    {
        _dispatcher = dispatcher;
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<UserStreetAddress> UserStreetAddresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Users");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        /*
        * This last one just make sure that all our prices are going to be stored in the database with a precision of
        * 18 comma 6, so it'll be consistent for all of our columns, which for now is just 1
        */
        configurationBuilder.Properties<decimal>().HavePrecision(18, 6);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        if (_dispatcher == null) return result;

        // dispatch events only if save was successful
        var entitiesWithEvents = ChangeTracker.Entries<IHaveDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
        .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

        return result;
    }
}
