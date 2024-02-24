namespace RiverBooks.Users;

/*
* We could just use the DbContext directly here, but I personally prefer to use a simple abstraction. It makes it so it's
* much easier for us to add a caching layer or a number of other things in the future.
*/
public interface IApplicationUserRepository
{
    Task<ApplicationUser> GetUserWithCardByEmailAsync(string email);
    Task<int> SaveChangesAsync();
}
