using Cityinfo.API.Entities;

namespace Cityinfo.API.Services.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUser(int userId);
    Task<User?> GetUser(string name);
    Task<bool> UserExists(string name);
    User AddUser(User user);
    Task SaveChangesAsync();
}
