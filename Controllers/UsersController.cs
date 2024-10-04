using Asp.Versioning;
using AutoMapper;
using Cityinfo.API.Entities;
using Cityinfo.API.Models;
using Cityinfo.API.Services;
using Cityinfo.API.Services.Repositories;
using Cityinfo.API.utilities;
using Microsoft.AspNetCore.Mvc;

namespace Cityinfo.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion(1)]
public class UsersController(IUserRepository userRepository, IMapper mapper, ITokenManager tokenManager)
    : ControllerBase
{
    [HttpPost("signup")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ResponseError>(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Signup(RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userExists = await userRepository.UserExists(registerRequest.UserName);

        if (userExists)
        {
            return Conflict(new ResponseError("User already exists", StatusCodes.Status409Conflict));
        }

        var newUser = mapper.Map<User>(registerRequest);
        newUser.Role = Role.User;

        userRepository.AddUser(newUser);
        await userRepository.SaveChangesAsync();

        var response = mapper.Map<UserResponse>(newUser);

        var token = tokenManager.AssignToken(response.Id, response.Name, response.role);

        Response.Headers.Append("token", token);

        return Ok(response);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var findUser = await userRepository.GetUser(registerRequest.UserName);

        if (findUser is null)
        {
            return BadRequest(new ResponseError("Invalid Credentials", StatusCodes.Status400BadRequest));
        }

        var isPasswordValid = registerRequest.Password.ToVerifyHash(findUser.Password);

        if (!isPasswordValid)
        {
            return BadRequest(new ResponseError("Invalid Credentials", StatusCodes.Status400BadRequest));
        }

        var token = tokenManager.AssignToken(findUser.Id, findUser.Name, findUser.Role);

        Response.Headers.Append("token", token);

        return NoContent();
    }
}
