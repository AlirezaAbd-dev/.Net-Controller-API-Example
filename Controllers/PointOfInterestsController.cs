using Asp.Versioning;
using AutoMapper;
using Cityinfo.API.Entities;
using Cityinfo.API.Models;
using Cityinfo.API.Services;
using Cityinfo.API.Services.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Cityinfo.API.Controllers {
    [ApiController]
    [Route("api/v{version:apiVersion}/cities/{cityId:int}/pointsOfInterest")]
    [ApiVersion(2)]
    public class PointOfInterestsController(
        ILogger<PointOfInterestsController> logger,
        IMailService mailService,
        ICityInfoRepository cityInfoRepository,
        IMapper mapper
        ) : ControllerBase {

        #region Services
        private readonly ILogger<PointOfInterestsController> _logger = logger ?? throw new ArgumentNullException();
        private readonly IMailService _mailService = mailService ?? throw new ArgumentNullException();
        private readonly ICityInfoRepository _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException();
        private readonly IMapper _mapper = mapper ?? throw new ArgumentException();
        #endregion

        #region Get
        [HttpGet]
        [ProducesResponseType<IEnumerable<PointOfInterestDto>>(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetPointsOfInterests(int cityId) {
            try {
                var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);

                if( cityExists is false ) {
                    _logger.LogInformation($"City with cityId {cityId} was not found");
                    return NotFound();
                }

                var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
            }
            catch( Exception ex ) {
                _logger.LogCritical($"Exception getting {cityId}", ex);
                return StatusCode(500, "A problem happened while ...");
            }

        }

        [HttpGet("{id:int}", Name = "GetPointOfInterest")]
        [ProducesResponseType<PointOfInterestDto>(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetPointsOfInterest(int cityId, int id) {
            var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);

            if( cityExists is false ) {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, id);

            if( pointOfInterest is null ) {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }
        #endregion

        #region Post
        [HttpPost]
        [ProducesResponseType<PointOfInterestDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreatePointOfInterest(
            int cityId,
            PointOfInterestForCreationDto pointOfInterest
        ) {

            if( !ModelState.IsValid ) {
                return BadRequest();
            }

            if( !await _cityInfoRepository.CityExistsAsync(cityId) ) {
                return NotFound();
            }

            var finalPoint = _mapper.Map<PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPoint);
            await _cityInfoRepository.SaveChangesAsync();

            var createdPoint = _mapper.Map<PointOfInterestDto>(finalPoint);

            return CreatedAtRoute(
                "GetPointOfInterest",
                new {
                    cityId = cityId,
                    id = createdPoint.Id
                },
                createdPoint
            );
        }
        #endregion

        #region Put
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePointOfInterest(int id, int cityId, PointOfInterestForUpdateDto pointOfInterest) {
            if( !ModelState.IsValid ) {
                return BadRequest();
            }

            if( !await _cityInfoRepository.CityExistsAsync(cityId) ) {
                return NotFound();
            }

            var point = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, id);

            if( point is null ) {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, point);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Patch
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int id, int cityId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
            ) {
            if( !await _cityInfoRepository.CityExistsAsync(cityId) ) {
                return NotFound();
            }

            var pointEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, id);

            if( pointEntity is null ) {
                return NotFound();
            }

            var pointToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointEntity);

            patchDocument.ApplyTo(pointToPatch,ModelState);

            if( !ModelState.IsValid ) {
                return BadRequest();
            }

            if( !TryValidateModel(pointToPatch) ) {
                return BadRequest();
            }

            _mapper.Map(pointToPatch, pointEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deletepointofinterest(int cityId, int id) {

            if( !await _cityInfoRepository.CityExistsAsync(cityId) ) {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, id);

            if( pointOfInterestEntity is null ) {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterestForCity(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send("Delete Point of interest", $"point of interest with id {id} has been deleted");
            return NoContent();
        }
        #endregion
    }
}
