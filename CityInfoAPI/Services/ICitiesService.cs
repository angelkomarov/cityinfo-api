using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICitiesService
    {
        #region sync

        IEnumerable<CityWithoutPointsOfInterestDto> GetCities();
        CityWithoutPointsOfInterestDto GetCity(int cityId, bool includePointsOfInterest);

        #endregion

        #region async
        Task<IEnumerable<CityDto>> GetAllCityInfoAsync(CancellationToken cancellationToken);
        Task<IEnumerable<CityWithoutPointsOfInterestDto>> GetCitiesAsync(CancellationToken cancellationToken);
        Task<CityWithoutPointsOfInterestDto> GetCityAsync(int cityId, bool includePointsOfInterest, CancellationToken cancellationToken);

        #endregion
    }
}
