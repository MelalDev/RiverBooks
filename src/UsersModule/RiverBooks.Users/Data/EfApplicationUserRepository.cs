using Microsoft.EntityFrameworkCore;

namespace RiverBooks.Users.Data;

internal class EfApplicationUserRepository : IApplicationUserRepository
{
    private readonly UsersDbContext _dbContext;

    public EfApplicationUserRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApplicationUser> GetUserWithAddressesByEmailAsync(string email)
    {
        return await _dbContext.ApplicationUsers
            .Include(user => user.Addresses)
            .SingleAsync(user => user.Email == email);
    }

    public async Task<ApplicationUser> GetUserWithCardByEmailAsync(string email)
    {
        return await _dbContext.ApplicationUsers
            .Include(user => user.CartItems)
            .SingleAsync(user => user.Email == email);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}
