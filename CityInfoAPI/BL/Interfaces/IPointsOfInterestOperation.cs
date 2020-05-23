using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfoAPI.BL.Interfaces
{
    public interface IPointsOfInterestOperation
    {
        #region sync

        IEnumerable<PointOfInterestDto> GetPointsOfInterestForCity(int cityId);
        PointOfInterestDto GetPointOfInterestForCity(int cityId, int pointOfInterestId);
        PointOfInterestDto AddPointOfInterestForCity(int cityId, PointOfInterestForCreationDto pointOfInterestDto);
        bool UpdatePointOfInterest(int cityId, int id, PointOfInterestForUpdateDto pointOfInterestDto);
        void PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto);
        bool DeletePointOfInterest(int cityId, int id);

        #endregion

        #region async

        Task<IEnumerable<PointOfInterestDto>> GetPointsOfInterestForCityAsync(int cityId, CancellationToken cancellationToken);
        Task<PointOfInterestDto> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId, CancellationToken cancellationToken);
        Task<PointOfInterestDto> AddPointOfInterestForCityAsync(int cityId, PointOfInterestForCreationDto pointOfInterestDto, CancellationToken cancellationToken);
        Task<bool> UpdatePointOfInterestAsync(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto, CancellationToken cancellationToken);
        Task PartiallyUpdatePointOfInterestAsync(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto, CancellationToken cancellationToken);
        Task<bool> DeletePointOfInterestAsync(int cityId, int pointOfInterestId, CancellationToken cancellationToken);

        #endregion

    }
}
