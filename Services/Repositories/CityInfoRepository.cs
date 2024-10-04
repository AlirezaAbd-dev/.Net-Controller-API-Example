using Cityinfo.API.DbContexts;
using Cityinfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cityinfo.API.Services.Repositories {
    public class CityInfoRepository(CityInfoDbContext context) : ICityInfoRepository {
        public async Task<bool> CityExistsAsync(int cityId) {
            return await context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(int userId) {
            var user = await context.Users.Include(u => u.Cities).FirstOrDefaultAsync(u => u.Id == userId);
            return user?.Cities.ToList() ?? [];
        }

        public async Task<City?> GetCityAsync(int userId, int id, bool includePointOfInterest = false) {
            User? user;

            if( includePointOfInterest ) {
                user = await context.Users.Include(u => u.Cities).ThenInclude(c=>c.PointOfInterest).FirstOrDefaultAsync(u => u.Id == userId);
                return await context.Cities.FirstOrDefaultAsync(c => c.Id == id);
            }

            user = await context.Users.Include(u => u.Cities).FirstOrDefaultAsync(u => u.Id == userId);

            return await context.Cities
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId) {
            return await context.PointsOfInterest
                .FirstOrDefaultAsync(p => p.Id == pointOfInterestId && p.CityId == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId) {
            return await context.PointsOfInterest
                .OrderBy(p => p.Name)
                .Where(p => p.CityId == cityId).ToListAsync();

        }

        public async Task AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest) {
            var city = await GetCityAsync(1,cityId, false);

            city?.PointOfInterest.Add(pointOfInterest);
        }

        public async Task<bool> SaveChangesAsync() {
            return ( await context.SaveChangesAsync() > 0 );
        }

        public void DeletePointOfInterestForCity(PointOfInterest pointOfInterest) {
            context.PointsOfInterest.Remove(pointOfInterest);
        }
    }
}
