using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;

namespace Cityinfo.API.Controllers {
    [Route("api/v{version:apiVersion}/authentication")]
    [ApiController]
    [ApiVersion(1)]
    public class AuthenticationController(IConfiguration config) : ControllerBase {

        public class AuthenticationRequestBody {
            public string? UserName {
                get; set;
            }
            public string? Password {
                get; set;
            }
        }

        public class CityInfoUser {
            public int UserId {
                get; set;
            }
            public string UserName {
                get; set;
            }
            public string FirstName {
                get; set;
            }
            public string LastName {
                get; set;
            }
            public string City {
                get; set;
            }

            public CityInfoUser(int userId, string username, string firstName, string lastName, string city) {
                UserId = userId;
                UserName = username;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody) {
            var user = ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);


            if( user == null ) {
                return Unauthorized();
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(config["Authentication:SecretForKey"]!)
            );
            var signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );
            var claimsForToken = new List<Claim> {
                new Claim("userId", user.UserId.ToString()),
                new Claim("firstName", user.UserName.ToString()),
                new Claim("city", user.City.ToString())
            };

            var jwtSecurityToken = new JwtSecurityToken(
                config["Authentication:Issuer"],
                config["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials
            );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? username, string? password) {
            return new CityInfoUser(1, username ?? "", "Alireza", "Abedi", "Khomam");
        }
    }
}
