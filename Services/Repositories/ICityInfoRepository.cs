using Cityinfo.API.Entities;

namespace Cityinfo.API.Services.Repositories {
    public interface ICityInfoRepository {
        Task<IEnumerable<City>> GetCitiesAsync(int userId);
        Task<City?> GetCityAsync(int userId, int id, bool includePointOfInterest);
        Task<bool> CityExistsAsync(int cityId);

        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
        Task AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest);
        void DeletePointOfInterestForCity(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
    }
}
