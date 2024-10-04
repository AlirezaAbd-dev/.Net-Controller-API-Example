namespace Cityinfo.API.Models;

public record ResponseError(string message,int status, string? type = "Error");
