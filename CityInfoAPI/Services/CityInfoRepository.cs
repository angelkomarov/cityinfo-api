using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CityInfo.API.Entities;


namespace CityInfo.API.Services
{
    //!!AK5 - implement repository layer
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;
        //!!AK5.2 inject DBContext (though inside other extracts could be used - e.g. call another service)
        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        #region sync

        public bool CityExists(int cityId)
        {
            return _context.Cities.Any(c => c.Id == cityId);
        }
        public IEnumerable<City> GetCities()
        {
            //!!AK5.3 - ToList mean that the query has to be execured right here!!
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }
        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                //!!AK5.4 LINQ Include = get child records. 
                //FirstOrDefault executes the query
                return _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefault();
            }

            return _context.Cities.Where(c => c.Id == cityId).FirstOrDefault();
        }
        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _context.PointsOfInterest
                           .Where(p => p.CityId == cityId).ToList();
        }
        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _context.PointsOfInterest
               .Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefault();
        }
        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }
        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }
        //!!AK5.5 - save changes to DB.
        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        #endregion

        #region async

        public async Task<IEnumerable<City>> GetCitiesAsync(CancellationToken cancellationToken)
        {
            //!!AK5.3 - ToList mean that the query has to be execured right here!!
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync(cancellationToken);
        }

        public async Task<bool> CityExistsAsync(int cityId, CancellationToken cancellationToken)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId, cancellationToken);
        }

        public async Task<City> GetCityAsync(int cityId, bool includePointsOfInterest, CancellationToken cancellationToken)
        {
            if (includePointsOfInterest)
            {
                //!!AK5.4 LINQ Include = get child records. 
                //FirstOrDefault executes the query
                return await _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync(cancellationToken);
            }

            return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<City>> GetAllCityInfoAsync(CancellationToken cancellationToken)
        {
            return await _context.Cities.Include(c => c.PointsOfInterest).OrderBy(c => c.Name).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId, CancellationToken cancellationToken)
        {
            return await _context.PointsOfInterest
                           .Where(p => p.CityId == cityId).ToListAsync(cancellationToken);
        }
        public async Task<PointOfInterest> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId, CancellationToken cancellationToken)
        {
            return await _context.PointsOfInterest
               .Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest, CancellationToken cancellationToken)
        {
            var city = await GetCityAsync(cityId, false, cancellationToken);
            city.PointsOfInterest.Add(pointOfInterest);
        }
        public async Task<bool> DeletePointOfInterestAsync(PointOfInterest pointOfInterest, CancellationToken cancellationToken)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
            return (await _context.SaveChangesAsync(cancellationToken) >= 0);
        }

        //!!AK5.5 - save changes to DB.~
        public async Task<bool> SaveAsync(CancellationToken cancellationToken)
        {
            return (await _context.SaveChangesAsync(cancellationToken) >= 0);
        }


        #endregion
    }
}
