using Cityinfo.API.Entities;

namespace Cityinfo.API.Services;

public interface ITokenManager
{
    string AssignToken(int id, string name, Role role);
}