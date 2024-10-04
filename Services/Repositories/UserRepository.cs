using Cityinfo.API.DbContexts;
using Cityinfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cityinfo.API.Services.Repositories;

public class UserRepository(CityInfoDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User?> GetUser(int userId)
    {
        return await context.Users.FirstOrDefaultAsync(u=> u.Id == userId);
    }

    public async Task<User?> GetUser(string name)
    {
        return await context.Users.FirstOrDefaultAsync(u=> u.Name == name);
    }

    public async Task<bool> UserExists(string name)
    {
        return await context.Users.AnyAsync(u => u.Name == name);
    }

    public User AddUser(User user)
    {
        context.Users.Add(user);

        return user;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
