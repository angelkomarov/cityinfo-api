using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    //!!AK5 - implement repository layer
    public interface ICityInfoRepository
    {
        # region sync
        bool CityExists(int cityId);
        //!!AK5.1 - return IEnumerable rather than IQueryable - not to leak persistence related logic!
        //consumer can keep building on it (it can extend with .where, .orderby) possibly before the query is executed 
        IEnumerable<City> GetCities();
        City GetCity(int cityId, bool includePointsOfInterest);
        IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId);
        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);
        void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest);
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
        bool Save();

        #endregion

        # region async

        Task<bool> CityExistsAsync(int cityId, CancellationToken cancellationToken);
        Task<IEnumerable<City>> GetCitiesAsync(CancellationToken cancellationToken);
        Task<City> GetCityAsync(int cityId, bool includePointsOfInterest, CancellationToken cancellationToken);
        Task<IEnumerable<City>> GetAllCityInfoAsync(CancellationToken cancellationToken);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId, CancellationToken cancellationToken);
        Task<PointOfInterest> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId, CancellationToken cancellationToken);
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest, CancellationToken cancellationToken);
        Task<bool> DeletePointOfInterestAsync(PointOfInterest pointOfInterest, CancellationToken cancellationToken);
        Task<bool> SaveAsync(CancellationToken cancellationToken);

        #endregion
    }
}
