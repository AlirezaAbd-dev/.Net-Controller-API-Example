using Cityinfo.API.Entities;

namespace Cityinfo.API.Models;

public class UserResponse
{
    public string Name { get; set; }
    public int Id { get; set; }
    public Role role { get; set; }
}
