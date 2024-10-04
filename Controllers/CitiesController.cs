using Asp.Versioning;
using AutoMapper;
using Cityinfo.API.Models;
using Cityinfo.API.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cityinfo.API.Controllers {
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/cities")]
    [ApiVersion(1)]
    [ApiVersion(2)]
    public class CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper) : ControllerBase {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities() {
            var userId = User.Identities.First()?.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if( userId is null ) {
                return Unauthorized();
            }

            var cities = await cityInfoRepository.GetCitiesAsync(Convert.ToInt32(userId));

            var result = mapper.Map<List<CityWithoutPointOfInterestDto>>(cities);

            return Ok(result);
        }


        /// <summary>
        ///     get one city by it's ID
        /// </summary>
        /// <param name="id">city id</param>
        /// <param name="includePointOfInterest">point of interest id</param>
        /// <returns>it returns a city with the provided id</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType<CityWithoutPointOfInterestDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CityWithoutPointOfInterestDto>> GetCity(int id, bool includePointOfInterest = false) {
            var userId = User.Identities.First()?.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if( userId is null ) {
                return Unauthorized();
            }

            var city = await cityInfoRepository.GetCityAsync(Convert.ToInt32(userId), id, includePointOfInterest);

            if( city == null ) {
                return NotFound();
            }

            if( includePointOfInterest ) {
                var result = mapper.Map<CityDto>(city);
                return Ok(result);
            }
            else {
                var result = mapper.Map<CityWithoutPointOfInterestDto>(city);
                return Ok(result);
            }

        }
    }
}
