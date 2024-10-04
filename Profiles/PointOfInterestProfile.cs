using AutoMapper;
using Cityinfo.API.Entities;
using Cityinfo.API.Models;

namespace Cityinfo.API.Profiles {
    public class PointOfInterestProfile:Profile {
        public PointOfInterestProfile()
        {
            CreateMap<PointOfInterest, PointOfInterestDto>();
            CreateMap<PointOfInterestForCreationDto, PointOfInterest>();
            CreateMap<PointOfInterestForUpdateDto, PointOfInterest>();
            CreateMap<PointOfInterest, PointOfInterestForUpdateDto>();
        }
    }
}
