using AutoMapper;
using Cityinfo.API.Entities;
using Cityinfo.API.Models;

namespace Cityinfo.API.Profiles {
    public class CityProfile :Profile{
        public CityProfile()
        {
            CreateMap<City,CityWithoutPointOfInterestDto>();
            CreateMap<City,CityDto>();
        }
    }
}
